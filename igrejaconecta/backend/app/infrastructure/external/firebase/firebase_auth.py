"""
Firebase Authentication client
"""

import json
import base64
from typing import Optional, Dict, Any
import firebase_admin
from firebase_admin import credentials, auth
from app.application.interfaces.services.firebase_service import IFirebaseService
from app.core.config import settings
from app.core.exceptions import AuthenticationException


class FirebaseAuth(IFirebaseService):
    """Firebase Authentication service implementation"""
    
    def __init__(self):
        """Initialize Firebase Admin SDK"""
        if not firebase_admin._apps:
            self._initialize_firebase()
    
    def _initialize_firebase(self) -> None:
        """Initialize Firebase Admin SDK"""
        try:
            if settings.FIREBASE_CREDENTIALS_PATH:
                cred = credentials.Certificate(settings.FIREBASE_CREDENTIALS_PATH)
            elif settings.FIREBASE_CREDENTIALS_JSON:
                # Decode base64 JSON if provided
                cred_json = base64.b64decode(settings.FIREBASE_CREDENTIALS_JSON).decode('utf-8')
                cred_dict = json.loads(cred_json)
                cred = credentials.Certificate(cred_dict)
            elif settings.FIREBASE_PROJECT_ID:
                # Use default credentials (for GCP environments)
                cred = credentials.ApplicationDefault()
            else:
                raise ValueError("Firebase credentials not configured")
            
            firebase_admin.initialize_app(cred)
        except Exception as e:
            raise AuthenticationException(f"Failed to initialize Firebase: {str(e)}")
    
    def verify_token(self, token: str) -> Optional[Dict[str, Any]]:
        """Verify Firebase ID token and return decoded token"""
        try:
            decoded_token = auth.verify_id_token(token)
            return decoded_token
        except auth.InvalidIdTokenError:
            raise AuthenticationException("Invalid Firebase token")
        except auth.ExpiredIdTokenError:
            raise AuthenticationException("Firebase token expired")
        except Exception as e:
            raise AuthenticationException(f"Error verifying token: {str(e)}")
    
    def get_user_by_uid(self, uid: str) -> Optional[Dict[str, Any]]:
        """Get user information by Firebase UID"""
        try:
            user = auth.get_user(uid)
            return {
                'uid': user.uid,
                'email': user.email,
                'display_name': user.display_name,
                'email_verified': user.email_verified,
            }
        except auth.UserNotFoundError:
            return None
        except Exception as e:
            raise AuthenticationException(f"Error getting user: {str(e)}")
    
    def create_custom_token(self, uid: str, claims: Optional[Dict[str, Any]] = None) -> str:
        """Create custom token for user"""
        try:
            return auth.create_custom_token(uid, claims)
        except Exception as e:
            raise AuthenticationException(f"Error creating custom token: {str(e)}")


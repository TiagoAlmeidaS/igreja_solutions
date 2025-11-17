"""
Firebase authentication middleware
"""

from typing import Optional
from fastapi import HTTPException, Security, Depends
from fastapi.security import HTTPBearer, HTTPAuthorizationCredentials
from app.application.interfaces.services.firebase_service import IFirebaseService
from app.core.dependencies import get_firebase_service
from app.core.exceptions import AuthenticationException

security = HTTPBearer()


async def get_current_user(
    credentials: HTTPAuthorizationCredentials = Security(security),
    firebase_service: IFirebaseService = Depends(get_firebase_service)
) -> dict:
    """Get current authenticated user from Firebase token"""
    if not firebase_service:
        raise HTTPException(status_code=500, detail="Firebase service not configured")
    
    token = credentials.credentials
    
    try:
        decoded_token = firebase_service.verify_token(token)
        return decoded_token
    except AuthenticationException as e:
        raise HTTPException(status_code=401, detail=str(e))


async def get_firebase_uid(
    credentials: HTTPAuthorizationCredentials = Security(security),
    firebase_service: IFirebaseService = Depends(get_firebase_service)
) -> str:
    """Get Firebase UID from token"""
    user = await get_current_user(credentials, firebase_service)
    return user.get('uid')


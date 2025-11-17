"""
Firebase service interface
"""

from abc import ABC, abstractmethod
from typing import Optional, Dict, Any


class IFirebaseService(ABC):
    """Interface for Firebase service"""
    
    @abstractmethod
    def verify_token(self, token: str) -> Optional[Dict[str, Any]]:
        """Verify Firebase ID token and return decoded token"""
        pass
    
    @abstractmethod
    def get_user_by_uid(self, uid: str) -> Optional[Dict[str, Any]]:
        """Get user information by Firebase UID"""
        pass
    
    @abstractmethod
    def create_custom_token(self, uid: str, claims: Optional[Dict[str, Any]] = None) -> str:
        """Create custom token for user"""
        pass


"""
Church repository interface
"""

from abc import ABC, abstractmethod
from typing import Optional, List
from app.domain.entities.church import Church


class IChurchRepository(ABC):
    """Interface for church repository"""
    
    @abstractmethod
    def create(self, church: Church) -> Church:
        """Create a new church"""
        pass
    
    @abstractmethod
    def get_by_id(self, church_id: int) -> Optional[Church]:
        """Get church by ID"""
        pass
    
    @abstractmethod
    def get_by_email(self, email: str) -> Optional[Church]:
        """Get church by email"""
        pass
    
    @abstractmethod
    def get_by_firebase_uid(self, firebase_uid: str) -> Optional[Church]:
        """Get church by Firebase UID"""
        pass
    
    @abstractmethod
    def update(self, church: Church) -> Church:
        """Update church"""
        pass
    
    @abstractmethod
    def delete(self, church_id: int) -> None:
        """Delete church"""
        pass
    
    @abstractmethod
    def list_all(self, skip: int = 0, limit: int = 100) -> List[Church]:
        """List all churches"""
        pass


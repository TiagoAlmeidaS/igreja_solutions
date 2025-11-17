"""
Contact repository interface
"""

from abc import ABC, abstractmethod
from typing import Optional, List
from app.domain.entities.contact import Contact


class IContactRepository(ABC):
    """Interface for contact repository"""
    
    @abstractmethod
    def create(self, contact: Contact) -> Contact:
        """Create a new contact"""
        pass
    
    @abstractmethod
    def get_by_id(self, contact_id: int) -> Optional[Contact]:
        """Get contact by ID"""
        pass
    
    @abstractmethod
    def get_by_phone(self, church_id: int, phone: str) -> Optional[Contact]:
        """Get contact by phone number"""
        pass
    
    @abstractmethod
    def list_by_church(self, church_id: int, skip: int = 0, limit: int = 100) -> List[Contact]:
        """List contacts by church"""
        pass
    
    @abstractmethod
    def list_by_tags(self, church_id: int, tags: List[str]) -> List[Contact]:
        """List contacts by tags"""
        pass
    
    @abstractmethod
    def update(self, contact: Contact) -> Contact:
        """Update contact"""
        pass
    
    @abstractmethod
    def delete(self, contact_id: int) -> None:
        """Delete contact"""
        pass
    
    @abstractmethod
    def bulk_create(self, contacts: List[Contact]) -> List[Contact]:
        """Bulk create contacts"""
        pass


"""
Broadcast repository interface
"""

from abc import ABC, abstractmethod
from typing import Optional, List
from datetime import datetime
from app.domain.entities.broadcast import Broadcast, BroadcastStatus


class IBroadcastRepository(ABC):
    """Interface for broadcast repository"""
    
    @abstractmethod
    def create(self, broadcast: Broadcast) -> Broadcast:
        """Create a new broadcast"""
        pass
    
    @abstractmethod
    def get_by_id(self, broadcast_id: int) -> Optional[Broadcast]:
        """Get broadcast by ID"""
        pass
    
    @abstractmethod
    def list_by_church(
        self, 
        church_id: int, 
        skip: int = 0, 
        limit: int = 100,
        status: Optional[BroadcastStatus] = None
    ) -> List[Broadcast]:
        """List broadcasts by church"""
        pass
    
    @abstractmethod
    def list_scheduled(self, before: Optional[datetime] = None) -> List[Broadcast]:
        """List scheduled broadcasts"""
        pass
    
    @abstractmethod
    def update(self, broadcast: Broadcast) -> Broadcast:
        """Update broadcast"""
        pass
    
    @abstractmethod
    def delete(self, broadcast_id: int) -> None:
        """Delete broadcast"""
        pass
    
    @abstractmethod
    def get_statistics(self, church_id: int) -> dict:
        """Get broadcast statistics for church"""
        pass


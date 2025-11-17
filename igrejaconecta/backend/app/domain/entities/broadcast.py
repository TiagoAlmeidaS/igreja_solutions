"""
Broadcast domain entity
"""

from dataclasses import dataclass
from datetime import datetime
from typing import Optional, List
from enum import Enum


class BroadcastStatus(Enum):
    """Broadcast status enumeration"""
    PENDING = "pending"
    SENT = "sent"
    FAILED = "failed"
    CANCELLED = "cancelled"


@dataclass
class Broadcast:
    """Broadcast domain entity"""
    id: Optional[int]
    church_id: int
    title: Optional[str]
    message: str
    link_url: Optional[str]
    button_text: Optional[str]
    contact_tags: List[str]
    scheduled_at: Optional[datetime]
    sent_at: Optional[datetime]
    status: BroadcastStatus
    total_sent: int
    created_at: datetime
    
    def __post_init__(self):
        """Validate entity after initialization"""
        if not self.church_id:
            raise ValueError("Church ID is required")
        if not self.message:
            raise ValueError("Message is required")
        if isinstance(self.status, str):
            self.status = BroadcastStatus(self.status)
    
    def schedule(self, scheduled_at: datetime) -> None:
        """Schedule broadcast for a specific time"""
        if scheduled_at <= datetime.utcnow():
            raise ValueError("Scheduled time must be in the future")
        self.scheduled_at = scheduled_at
        self.status = BroadcastStatus.PENDING
    
    def send(self) -> None:
        """Mark broadcast as sent"""
        self.status = BroadcastStatus.SENT
        self.sent_at = datetime.utcnow()
    
    def fail(self) -> None:
        """Mark broadcast as failed"""
        self.status = BroadcastStatus.FAILED
    
    def cancel(self) -> None:
        """Cancel scheduled broadcast"""
        if self.status == BroadcastStatus.SENT:
            raise ValueError("Cannot cancel already sent broadcast")
        self.status = BroadcastStatus.CANCELLED
    
    def update_total_sent(self, count: int) -> None:
        """Update total sent count"""
        if count < 0:
            raise ValueError("Total sent cannot be negative")
        self.total_sent = count
    
    def is_scheduled(self) -> bool:
        """Check if broadcast is scheduled"""
        return self.scheduled_at is not None and self.status == BroadcastStatus.PENDING
    
    def can_be_sent(self) -> bool:
        """Check if broadcast can be sent"""
        return self.status == BroadcastStatus.PENDING


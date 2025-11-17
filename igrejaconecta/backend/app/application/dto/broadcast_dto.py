"""
Broadcast DTOs
"""

from pydantic import BaseModel
from typing import Optional, List
from datetime import datetime


class BroadcastCreateDTO(BaseModel):
    """DTO for creating a broadcast"""
    title: Optional[str] = None
    message: str
    link_url: Optional[str] = None
    button_text: Optional[str] = None
    contact_tags: List[str] = []
    scheduled_at: Optional[datetime] = None


class BroadcastUpdateDTO(BaseModel):
    """DTO for updating a broadcast"""
    title: Optional[str] = None
    message: Optional[str] = None
    link_url: Optional[str] = None
    button_text: Optional[str] = None
    contact_tags: Optional[List[str]] = None
    scheduled_at: Optional[datetime] = None


class BroadcastResponseDTO(BaseModel):
    """DTO for broadcast response"""
    id: int
    church_id: int
    title: Optional[str]
    message: str
    link_url: Optional[str]
    button_text: Optional[str]
    contact_tags: List[str]
    scheduled_at: Optional[datetime]
    sent_at: Optional[datetime]
    status: str
    total_sent: int
    created_at: datetime
    
    class Config:
        from_attributes = True


class BroadcastFilterDTO(BaseModel):
    """DTO for filtering broadcasts"""
    status: Optional[str] = None
    skip: int = 0
    limit: int = 100


class BroadcastStatisticsDTO(BaseModel):
    """DTO for broadcast statistics"""
    total: int
    pending: int
    sent: int
    failed: int
    cancelled: int
    total_messages_sent: int


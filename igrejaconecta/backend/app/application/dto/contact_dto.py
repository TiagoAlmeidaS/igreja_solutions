"""
Contact DTOs
"""

from pydantic import BaseModel
from typing import Optional, List
from datetime import datetime


class ContactCreateDTO(BaseModel):
    """DTO for creating a contact"""
    name: Optional[str] = None
    phone: str
    tags: List[str] = []


class ContactUpdateDTO(BaseModel):
    """DTO for updating a contact"""
    name: Optional[str] = None
    phone: Optional[str] = None
    tags: Optional[List[str]] = None


class ContactResponseDTO(BaseModel):
    """DTO for contact response"""
    id: int
    church_id: int
    name: Optional[str]
    phone: str
    tags: List[str]
    created_at: datetime
    
    class Config:
        from_attributes = True


class ContactBulkCreateDTO(BaseModel):
    """DTO for bulk creating contacts"""
    contacts: List[ContactCreateDTO]


class ContactFilterDTO(BaseModel):
    """DTO for filtering contacts"""
    tags: Optional[List[str]] = None
    skip: int = 0
    limit: int = 100


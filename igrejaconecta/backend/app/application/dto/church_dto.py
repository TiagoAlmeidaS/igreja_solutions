"""
Church DTOs
"""

from pydantic import BaseModel, EmailStr
from typing import Optional
from datetime import datetime


class ChurchCreateDTO(BaseModel):
    """DTO for creating a church"""
    name: str
    admin_name: Optional[str] = None
    email: EmailStr
    phone: Optional[str] = None
    firebase_uid: str


class ChurchUpdateDTO(BaseModel):
    """DTO for updating a church"""
    name: Optional[str] = None
    admin_name: Optional[str] = None
    phone: Optional[str] = None


class ChurchResponseDTO(BaseModel):
    """DTO for church response"""
    id: int
    name: str
    admin_name: Optional[str]
    email: str
    phone: Optional[str]
    whatsapp_phone_id: Optional[str]
    is_active: bool
    created_at: datetime
    
    class Config:
        from_attributes = True


class WhatsAppConfigDTO(BaseModel):
    """DTO for configuring WhatsApp"""
    phone_id: str
    access_token: str


class WhatsAppConfigResponseDTO(BaseModel):
    """DTO for WhatsApp configuration response"""
    phone_id: str
    is_configured: bool


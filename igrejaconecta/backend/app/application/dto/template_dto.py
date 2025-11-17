"""
Template DTOs
"""

from pydantic import BaseModel
from typing import Optional, List
from datetime import datetime


class TemplateCreateDTO(BaseModel):
    """DTO for creating a template"""
    name: str
    message: str
    link_url: Optional[str] = None
    button_text: Optional[str] = None


class TemplateUpdateDTO(BaseModel):
    """DTO for updating a template"""
    name: Optional[str] = None
    message: Optional[str] = None
    link_url: Optional[str] = None
    button_text: Optional[str] = None


class TemplateResponseDTO(BaseModel):
    """DTO for template response"""
    id: int
    church_id: int
    name: str
    message: str
    link_url: Optional[str]
    button_text: Optional[str]
    created_at: datetime
    
    class Config:
        from_attributes = True


class UseTemplateDTO(BaseModel):
    """DTO for using a template in a broadcast"""
    template_id: int
    contact_tags: List[str] = []
    scheduled_at: Optional[datetime] = None


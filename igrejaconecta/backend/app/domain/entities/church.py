"""
Church domain entity
"""

from dataclasses import dataclass
from datetime import datetime
from typing import Optional


@dataclass
class Church:
    """Church domain entity"""
    id: Optional[int]
    name: str
    admin_name: Optional[str]
    email: str
    phone: Optional[str]
    whatsapp_phone_id: Optional[str]
    whatsapp_access_token: Optional[str]  # Encrypted
    created_at: datetime
    is_active: bool
    
    def __post_init__(self):
        """Validate entity after initialization"""
        if not self.name:
            raise ValueError("Church name is required")
        if not self.email:
            raise ValueError("Church email is required")
    
    def configure_whatsapp(self, phone_id: str, access_token: str) -> None:
        """Configure WhatsApp Business credentials"""
        if not phone_id:
            raise ValueError("WhatsApp phone ID is required")
        if not access_token:
            raise ValueError("WhatsApp access token is required")
        
        self.whatsapp_phone_id = phone_id
        self.whatsapp_access_token = access_token
    
    def is_whatsapp_configured(self) -> bool:
        """Check if WhatsApp is configured"""
        return bool(self.whatsapp_phone_id and self.whatsapp_access_token)
    
    def deactivate(self) -> None:
        """Deactivate church"""
        self.is_active = False
    
    def activate(self) -> None:
        """Activate church"""
        self.is_active = True


"""
Template domain entity
"""

from dataclasses import dataclass
from datetime import datetime
from typing import Optional


@dataclass
class Template:
    """Template domain entity"""
    id: Optional[int]
    church_id: int
    name: str
    message: str
    link_url: Optional[str]
    button_text: Optional[str]
    created_at: datetime
    
    def __post_init__(self):
        """Validate entity after initialization"""
        if not self.church_id:
            raise ValueError("Church ID is required")
        if not self.name:
            raise ValueError("Template name is required")
        if not self.message:
            raise ValueError("Template message is required")
    
    def update_message(self, message: str) -> None:
        """Update template message"""
        if not message:
            raise ValueError("Message cannot be empty")
        self.message = message
    
    def update_link(self, link_url: Optional[str], button_text: Optional[str] = None) -> None:
        """Update template link and button text"""
        self.link_url = link_url
        if button_text:
            self.button_text = button_text
    
    def update_name(self, name: str) -> None:
        """Update template name"""
        if not name:
            raise ValueError("Template name cannot be empty")
        self.name = name


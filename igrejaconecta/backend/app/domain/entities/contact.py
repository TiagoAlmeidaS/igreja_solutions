"""
Contact domain entity
"""

from dataclasses import dataclass
from datetime import datetime
from typing import Optional, List
from app.domain.value_objects.phone import Phone


@dataclass
class Contact:
    """Contact domain entity"""
    id: Optional[int]
    church_id: int
    name: Optional[str]
    phone: Phone
    tags: List[str]
    created_at: datetime
    
    def __post_init__(self):
        """Validate entity after initialization"""
        if not self.church_id:
            raise ValueError("Church ID is required")
        if not self.phone:
            raise ValueError("Phone is required")
    
    def add_tag(self, tag: str) -> None:
        """Add a tag to the contact"""
        if tag and tag not in self.tags:
            self.tags.append(tag)
    
    def remove_tag(self, tag: str) -> None:
        """Remove a tag from the contact"""
        if tag in self.tags:
            self.tags.remove(tag)
    
    def has_tag(self, tag: str) -> bool:
        """Check if contact has a specific tag"""
        return tag in self.tags
    
    def update_name(self, name: str) -> None:
        """Update contact name"""
        self.name = name
    
    def update_phone(self, phone: Phone) -> None:
        """Update contact phone"""
        self.phone = phone


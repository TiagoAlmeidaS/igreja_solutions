"""
Church model
"""

from sqlalchemy import Column, Integer, String, Boolean, DateTime, Text
from sqlalchemy.orm import relationship
from datetime import datetime
from app.core.database import Base


class Church(Base):
    __tablename__ = "churches"
    
    id = Column(Integer, primary_key=True, index=True)
    name = Column(String(100), nullable=False)
    admin_name = Column(String(100))
    email = Column(String(100), unique=True, nullable=False, index=True)
    password_hash = Column(Text, nullable=False)
    phone = Column(String(20))
    whatsapp_phone_id = Column(Text)
    whatsapp_access_token = Column(Text)  # Encrypted
    created_at = Column(DateTime, default=datetime.utcnow)
    is_active = Column(Boolean, default=True)
    
    # Relationships
    contacts = relationship("Contact", back_populates="church", cascade="all, delete-orphan")
    broadcasts = relationship("Broadcast", back_populates="church", cascade="all, delete-orphan")
    templates = relationship("Template", back_populates="church", cascade="all, delete-orphan")


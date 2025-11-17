"""
Church SQLAlchemy model
"""

from sqlalchemy import Column, Integer, String, Boolean, DateTime, Text
from sqlalchemy.orm import relationship
from datetime import datetime
from app.infrastructure.database.database import Base


class ChurchModel(Base):
    """Church SQLAlchemy model"""
    __tablename__ = "churches"
    
    id = Column(Integer, primary_key=True, index=True)
    name = Column(String(100), nullable=False)
    admin_name = Column(String(100))
    email = Column(String(100), unique=True, nullable=False, index=True)
    firebase_uid = Column(String(255), unique=True, nullable=True, index=True)
    phone = Column(String(20))
    whatsapp_phone_id = Column(Text)
    whatsapp_access_token = Column(Text)  # Encrypted
    created_at = Column(DateTime, default=datetime.utcnow)
    is_active = Column(Boolean, default=True)
    
    # Relationships
    contacts = relationship("ContactModel", back_populates="church", cascade="all, delete-orphan")
    broadcasts = relationship("BroadcastModel", back_populates="church", cascade="all, delete-orphan")
    templates = relationship("TemplateModel", back_populates="church", cascade="all, delete-orphan")


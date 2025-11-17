"""
Contact SQLAlchemy model
"""

from sqlalchemy import Column, Integer, String, DateTime, ForeignKey, ARRAY
from sqlalchemy.orm import relationship
from datetime import datetime
from app.infrastructure.database.database import Base


class ContactModel(Base):
    """Contact SQLAlchemy model"""
    __tablename__ = "contacts"
    
    id = Column(Integer, primary_key=True, index=True)
    church_id = Column(Integer, ForeignKey("churches.id"), nullable=False)
    name = Column(String(100))
    phone = Column(String(20), nullable=False)
    tags = Column(ARRAY(String))  # Array of tags
    created_at = Column(DateTime, default=datetime.utcnow)
    
    # Relationship
    church = relationship("ChurchModel", back_populates="contacts")


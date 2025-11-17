"""
Contact model
"""

from sqlalchemy import Column, Integer, String, DateTime, ForeignKey, ARRAY
from sqlalchemy.orm import relationship
from datetime import datetime
from app.core.database import Base


class Contact(Base):
    __tablename__ = "contacts"
    
    id = Column(Integer, primary_key=True, index=True)
    church_id = Column(Integer, ForeignKey("churches.id"), nullable=False)
    name = Column(String(100))
    phone = Column(String(20), nullable=False)
    tags = Column(ARRAY(String))  # Array of tags
    created_at = Column(DateTime, default=datetime.utcnow)
    
    # Relationship
    church = relationship("Church", back_populates="contacts")


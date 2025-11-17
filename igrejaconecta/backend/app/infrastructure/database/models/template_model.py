"""
Template SQLAlchemy model
"""

from sqlalchemy import Column, Integer, String, DateTime, ForeignKey, Text
from sqlalchemy.orm import relationship
from datetime import datetime
from app.infrastructure.database.database import Base


class TemplateModel(Base):
    """Template SQLAlchemy model"""
    __tablename__ = "templates"
    
    id = Column(Integer, primary_key=True, index=True)
    church_id = Column(Integer, ForeignKey("churches.id"), nullable=False)
    name = Column(String(50), nullable=False)
    message = Column(Text)
    link_url = Column(Text)
    button_text = Column(String(50))
    created_at = Column(DateTime, default=datetime.utcnow)
    
    # Relationship
    church = relationship("ChurchModel", back_populates="templates")


"""
Template model
"""

from sqlalchemy import Column, Integer, String, DateTime, ForeignKey, Text
from sqlalchemy.orm import relationship
from datetime import datetime
from app.core.database import Base


class Template(Base):
    __tablename__ = "templates"
    
    id = Column(Integer, primary_key=True, index=True)
    church_id = Column(Integer, ForeignKey("churches.id"), nullable=False)
    name = Column(String(50))
    message = Column(Text)
    link_url = Column(Text)
    button_text = Column(String(50))
    created_at = Column(DateTime, default=datetime.utcnow)
    
    # Relationship
    church = relationship("Church", back_populates="templates")


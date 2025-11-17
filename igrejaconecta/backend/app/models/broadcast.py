"""
Broadcast model
"""

from sqlalchemy import Column, Integer, String, DateTime, ForeignKey, Text, ARRAY
from sqlalchemy.orm import relationship
from datetime import datetime
from app.core.database import Base


class Broadcast(Base):
    __tablename__ = "broadcasts"
    
    id = Column(Integer, primary_key=True, index=True)
    church_id = Column(Integer, ForeignKey("churches.id"), nullable=False)
    title = Column(String(100))
    message = Column(Text)
    link_url = Column(Text)
    button_text = Column(String(50))
    contact_tags = Column(ARRAY(String))  # Array of tags to filter contacts
    scheduled_at = Column(DateTime)
    sent_at = Column(DateTime)
    status = Column(String(20), default="pending")  # pending, sent, failed
    total_sent = Column(Integer, default=0)
    created_at = Column(DateTime, default=datetime.utcnow)
    
    # Relationship
    church = relationship("Church", back_populates="broadcasts")


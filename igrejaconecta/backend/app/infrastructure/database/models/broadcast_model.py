"""
Broadcast SQLAlchemy model
"""

from sqlalchemy import Column, Integer, String, DateTime, ForeignKey, Text, ARRAY
from sqlalchemy.orm import relationship
from datetime import datetime
from app.infrastructure.database.database import Base


class BroadcastModel(Base):
    """Broadcast SQLAlchemy model"""
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
    status = Column(String(20), default="pending")  # pending, sent, failed, cancelled
    total_sent = Column(Integer, default=0)
    created_at = Column(DateTime, default=datetime.utcnow)
    
    # Relationship
    church = relationship("ChurchModel", back_populates="broadcasts")


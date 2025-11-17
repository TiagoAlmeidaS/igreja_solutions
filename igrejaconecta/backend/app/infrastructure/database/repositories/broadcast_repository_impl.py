"""
Broadcast repository implementation
"""

from typing import Optional, List
from datetime import datetime
from sqlalchemy.orm import Session
from sqlalchemy import func
from app.domain.entities.broadcast import Broadcast, BroadcastStatus
from app.application.interfaces.repositories.broadcast_repository import IBroadcastRepository
from app.infrastructure.database.models.broadcast_model import BroadcastModel
from app.core.exceptions import BroadcastNotFoundException, RepositoryException


class BroadcastRepositoryImpl(IBroadcastRepository):
    """Broadcast repository implementation"""
    
    def __init__(self, db: Session):
        self.db = db
    
    def _to_domain(self, model: BroadcastModel) -> Broadcast:
        """Convert SQLAlchemy model to domain entity"""
        return Broadcast(
            id=model.id,
            church_id=model.church_id,
            title=model.title,
            message=model.message,
            link_url=model.link_url,
            button_text=model.button_text,
            contact_tags=model.contact_tags or [],
            scheduled_at=model.scheduled_at,
            sent_at=model.sent_at,
            status=BroadcastStatus(model.status),
            total_sent=model.total_sent,
            created_at=model.created_at
        )
    
    def _to_model(self, entity: Broadcast) -> BroadcastModel:
        """Convert domain entity to SQLAlchemy model"""
        if entity.id:
            model = self.db.query(BroadcastModel).filter(BroadcastModel.id == entity.id).first()
            if not model:
                raise BroadcastNotFoundException(f"Broadcast with id {entity.id} not found")
        else:
            model = BroadcastModel()
        
        model.church_id = entity.church_id
        model.title = entity.title
        model.message = entity.message
        model.link_url = entity.link_url
        model.button_text = entity.button_text
        model.contact_tags = entity.contact_tags
        model.scheduled_at = entity.scheduled_at
        model.sent_at = entity.sent_at
        model.status = entity.status.value
        model.total_sent = entity.total_sent
        
        return model
    
    def create(self, broadcast: Broadcast) -> Broadcast:
        """Create a new broadcast"""
        try:
            model = self._to_model(broadcast)
            self.db.add(model)
            self.db.commit()
            self.db.refresh(model)
            return self._to_domain(model)
        except Exception as e:
            self.db.rollback()
            raise RepositoryException(f"Error creating broadcast: {str(e)}")
    
    def get_by_id(self, broadcast_id: int) -> Optional[Broadcast]:
        """Get broadcast by ID"""
        model = self.db.query(BroadcastModel).filter(BroadcastModel.id == broadcast_id).first()
        return self._to_domain(model) if model else None
    
    def list_by_church(
        self, 
        church_id: int, 
        skip: int = 0, 
        limit: int = 100,
        status: Optional[BroadcastStatus] = None
    ) -> List[Broadcast]:
        """List broadcasts by church"""
        query = self.db.query(BroadcastModel).filter(BroadcastModel.church_id == church_id)
        
        if status:
            query = query.filter(BroadcastModel.status == status.value)
        
        models = query.order_by(BroadcastModel.created_at.desc()).offset(skip).limit(limit).all()
        return [self._to_domain(model) for model in models]
    
    def list_scheduled(self, before: Optional[datetime] = None) -> List[Broadcast]:
        """List scheduled broadcasts"""
        query = self.db.query(BroadcastModel).filter(
            BroadcastModel.status == BroadcastStatus.PENDING.value,
            BroadcastModel.scheduled_at.isnot(None)
        )
        
        if before:
            query = query.filter(BroadcastModel.scheduled_at <= before)
        
        models = query.order_by(BroadcastModel.scheduled_at.asc()).all()
        return [self._to_domain(model) for model in models]
    
    def update(self, broadcast: Broadcast) -> Broadcast:
        """Update broadcast"""
        try:
            model = self._to_model(broadcast)
            self.db.commit()
            self.db.refresh(model)
            return self._to_domain(model)
        except Exception as e:
            self.db.rollback()
            raise RepositoryException(f"Error updating broadcast: {str(e)}")
    
    def delete(self, broadcast_id: int) -> None:
        """Delete broadcast"""
        try:
            model = self.db.query(BroadcastModel).filter(BroadcastModel.id == broadcast_id).first()
            if not model:
                raise BroadcastNotFoundException(f"Broadcast with id {broadcast_id} not found")
            self.db.delete(model)
            self.db.commit()
        except BroadcastNotFoundException:
            raise
        except Exception as e:
            self.db.rollback()
            raise RepositoryException(f"Error deleting broadcast: {str(e)}")
    
    def get_statistics(self, church_id: int) -> dict:
        """Get broadcast statistics for church"""
        stats = self.db.query(
            BroadcastModel.status,
            func.count(BroadcastModel.id).label('count'),
            func.sum(BroadcastModel.total_sent).label('total_sent')
        ).filter(
            BroadcastModel.church_id == church_id
        ).group_by(BroadcastModel.status).all()
        
        result = {
            'total': 0,
            'pending': 0,
            'sent': 0,
            'failed': 0,
            'cancelled': 0,
            'total_messages_sent': 0
        }
        
        for status, count, total_sent in stats:
            result[status] = count
            result['total'] += count
            if total_sent:
                result['total_messages_sent'] += total_sent
        
        return result


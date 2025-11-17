"""
Church repository implementation
"""

from typing import Optional, List
from sqlalchemy.orm import Session
from app.domain.entities.church import Church
from app.application.interfaces.repositories.church_repository import IChurchRepository
from app.infrastructure.database.models.church_model import ChurchModel
from app.core.exceptions import ChurchNotFoundException, RepositoryException
from datetime import datetime


class ChurchRepositoryImpl(IChurchRepository):
    """Church repository implementation"""
    
    def __init__(self, db: Session):
        self.db = db
    
    def _to_domain(self, model: ChurchModel) -> Church:
        """Convert SQLAlchemy model to domain entity"""
        if not model:
            return None
        return Church(
            id=model.id,
            name=model.name,
            admin_name=model.admin_name,
            email=model.email,
            phone=model.phone,
            whatsapp_phone_id=model.whatsapp_phone_id,
            whatsapp_access_token=model.whatsapp_access_token,
            created_at=model.created_at,
            is_active=model.is_active
        )
    
    def _to_model(self, entity: Church) -> ChurchModel:
        """Convert domain entity to SQLAlchemy model"""
        if entity.id:
            model = self.db.query(ChurchModel).filter(ChurchModel.id == entity.id).first()
            if not model:
                raise ChurchNotFoundException(f"Church with id {entity.id} not found")
        else:
            model = ChurchModel()
        
        model.name = entity.name
        model.admin_name = entity.admin_name
        model.email = entity.email
        model.phone = entity.phone
        model.whatsapp_phone_id = entity.whatsapp_phone_id
        model.whatsapp_access_token = entity.whatsapp_access_token
        model.is_active = entity.is_active
        # Note: firebase_uid should be set separately during creation
        
        return model
    
    def create(self, church: Church) -> Church:
        """Create a new church"""
        try:
            model = self._to_model(church)
            self.db.add(model)
            self.db.commit()
            self.db.refresh(model)
            return self._to_domain(model)
        except Exception as e:
            self.db.rollback()
            raise RepositoryException(f"Error creating church: {str(e)}")
    
    def get_by_id(self, church_id: int) -> Optional[Church]:
        """Get church by ID"""
        model = self.db.query(ChurchModel).filter(ChurchModel.id == church_id).first()
        return self._to_domain(model) if model else None
    
    def get_by_email(self, email: str) -> Optional[Church]:
        """Get church by email"""
        model = self.db.query(ChurchModel).filter(ChurchModel.email == email).first()
        return self._to_domain(model) if model else None
    
    def get_by_firebase_uid(self, firebase_uid: str) -> Optional[Church]:
        """Get church by Firebase UID"""
        model = self.db.query(ChurchModel).filter(ChurchModel.firebase_uid == firebase_uid).first()
        return self._to_domain(model) if model else None
    
    def update(self, church: Church) -> Church:
        """Update church"""
        try:
            model = self._to_model(church)
            self.db.commit()
            self.db.refresh(model)
            return self._to_domain(model)
        except Exception as e:
            self.db.rollback()
            raise RepositoryException(f"Error updating church: {str(e)}")
    
    def delete(self, church_id: int) -> None:
        """Delete church"""
        try:
            model = self.db.query(ChurchModel).filter(ChurchModel.id == church_id).first()
            if not model:
                raise ChurchNotFoundException(f"Church with id {church_id} not found")
            self.db.delete(model)
            self.db.commit()
        except ChurchNotFoundException:
            raise
        except Exception as e:
            self.db.rollback()
            raise RepositoryException(f"Error deleting church: {str(e)}")
    
    def list_all(self, skip: int = 0, limit: int = 100) -> List[Church]:
        """List all churches"""
        models = self.db.query(ChurchModel).offset(skip).limit(limit).all()
        return [self._to_domain(model) for model in models]


"""
Contact repository implementation
"""

from typing import Optional, List
from sqlalchemy.orm import Session
from sqlalchemy import and_
from app.domain.entities.contact import Contact
from app.domain.value_objects.phone import Phone
from app.application.interfaces.repositories.contact_repository import IContactRepository
from app.infrastructure.database.models.contact_model import ContactModel
from app.core.exceptions import ContactNotFoundException, RepositoryException


class ContactRepositoryImpl(IContactRepository):
    """Contact repository implementation"""
    
    def __init__(self, db: Session):
        self.db = db
    
    def _to_domain(self, model: ContactModel) -> Contact:
        """Convert SQLAlchemy model to domain entity"""
        return Contact(
            id=model.id,
            church_id=model.church_id,
            name=model.name,
            phone=Phone(model.phone),
            tags=model.tags or [],
            created_at=model.created_at
        )
    
    def _to_model(self, entity: Contact) -> ContactModel:
        """Convert domain entity to SQLAlchemy model"""
        if entity.id:
            model = self.db.query(ContactModel).filter(ContactModel.id == entity.id).first()
            if not model:
                raise ContactNotFoundException(f"Contact with id {entity.id} not found")
        else:
            model = ContactModel()
        
        model.church_id = entity.church_id
        model.name = entity.name
        model.phone = entity.phone.value
        model.tags = entity.tags
        
        return model
    
    def create(self, contact: Contact) -> Contact:
        """Create a new contact"""
        try:
            model = self._to_model(contact)
            self.db.add(model)
            self.db.commit()
            self.db.refresh(model)
            return self._to_domain(model)
        except Exception as e:
            self.db.rollback()
            raise RepositoryException(f"Error creating contact: {str(e)}")
    
    def get_by_id(self, contact_id: int) -> Optional[Contact]:
        """Get contact by ID"""
        model = self.db.query(ContactModel).filter(ContactModel.id == contact_id).first()
        return self._to_domain(model) if model else None
    
    def get_by_phone(self, church_id: int, phone: str) -> Optional[Contact]:
        """Get contact by phone number"""
        model = self.db.query(ContactModel).filter(
            and_(
                ContactModel.church_id == church_id,
                ContactModel.phone == phone
            )
        ).first()
        return self._to_domain(model) if model else None
    
    def list_by_church(self, church_id: int, skip: int = 0, limit: int = 100) -> List[Contact]:
        """List contacts by church"""
        models = self.db.query(ContactModel).filter(
            ContactModel.church_id == church_id
        ).offset(skip).limit(limit).all()
        return [self._to_domain(model) for model in models]
    
    def list_by_tags(self, church_id: int, tags: List[str]) -> List[Contact]:
        """List contacts by tags"""
        # PostgreSQL array overlap operator
        models = self.db.query(ContactModel).filter(
            and_(
                ContactModel.church_id == church_id,
                ContactModel.tags.op('&&')(tags)  # Array overlap
            )
        ).all()
        return [self._to_domain(model) for model in models]
    
    def update(self, contact: Contact) -> Contact:
        """Update contact"""
        try:
            model = self._to_model(contact)
            self.db.commit()
            self.db.refresh(model)
            return self._to_domain(model)
        except Exception as e:
            self.db.rollback()
            raise RepositoryException(f"Error updating contact: {str(e)}")
    
    def delete(self, contact_id: int) -> None:
        """Delete contact"""
        try:
            model = self.db.query(ContactModel).filter(ContactModel.id == contact_id).first()
            if not model:
                raise ContactNotFoundException(f"Contact with id {contact_id} not found")
            self.db.delete(model)
            self.db.commit()
        except ContactNotFoundException:
            raise
        except Exception as e:
            self.db.rollback()
            raise RepositoryException(f"Error deleting contact: {str(e)}")
    
    def bulk_create(self, contacts: List[Contact]) -> List[Contact]:
        """Bulk create contacts"""
        try:
            models = [self._to_model(contact) for contact in contacts]
            self.db.add_all(models)
            self.db.commit()
            for model in models:
                self.db.refresh(model)
            return [self._to_domain(model) for model in models]
        except Exception as e:
            self.db.rollback()
            raise RepositoryException(f"Error bulk creating contacts: {str(e)}")


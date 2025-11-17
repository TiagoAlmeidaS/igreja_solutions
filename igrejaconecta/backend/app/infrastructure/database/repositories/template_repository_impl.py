"""
Template repository implementation
"""

from typing import Optional, List
from sqlalchemy.orm import Session
from app.domain.entities.template import Template
from app.application.interfaces.repositories.template_repository import ITemplateRepository
from app.infrastructure.database.models.template_model import TemplateModel
from app.core.exceptions import TemplateNotFoundException, RepositoryException


class TemplateRepositoryImpl(ITemplateRepository):
    """Template repository implementation"""
    
    def __init__(self, db: Session):
        self.db = db
    
    def _to_domain(self, model: TemplateModel) -> Template:
        """Convert SQLAlchemy model to domain entity"""
        return Template(
            id=model.id,
            church_id=model.church_id,
            name=model.name,
            message=model.message,
            link_url=model.link_url,
            button_text=model.button_text,
            created_at=model.created_at
        )
    
    def _to_model(self, entity: Template) -> TemplateModel:
        """Convert domain entity to SQLAlchemy model"""
        if entity.id:
            model = self.db.query(TemplateModel).filter(TemplateModel.id == entity.id).first()
            if not model:
                raise TemplateNotFoundException(f"Template with id {entity.id} not found")
        else:
            model = TemplateModel()
        
        model.church_id = entity.church_id
        model.name = entity.name
        model.message = entity.message
        model.link_url = entity.link_url
        model.button_text = entity.button_text
        
        return model
    
    def create(self, template: Template) -> Template:
        """Create a new template"""
        try:
            model = self._to_model(template)
            self.db.add(model)
            self.db.commit()
            self.db.refresh(model)
            return self._to_domain(model)
        except Exception as e:
            self.db.rollback()
            raise RepositoryException(f"Error creating template: {str(e)}")
    
    def get_by_id(self, template_id: int) -> Optional[Template]:
        """Get template by ID"""
        model = self.db.query(TemplateModel).filter(TemplateModel.id == template_id).first()
        return self._to_domain(model) if model else None
    
    def list_by_church(self, church_id: int, skip: int = 0, limit: int = 100) -> List[Template]:
        """List templates by church"""
        models = self.db.query(TemplateModel).filter(
            TemplateModel.church_id == church_id
        ).order_by(TemplateModel.created_at.desc()).offset(skip).limit(limit).all()
        return [self._to_domain(model) for model in models]
    
    def update(self, template: Template) -> Template:
        """Update template"""
        try:
            model = self._to_model(template)
            self.db.commit()
            self.db.refresh(model)
            return self._to_domain(model)
        except Exception as e:
            self.db.rollback()
            raise RepositoryException(f"Error updating template: {str(e)}")
    
    def delete(self, template_id: int) -> None:
        """Delete template"""
        try:
            model = self.db.query(TemplateModel).filter(TemplateModel.id == template_id).first()
            if not model:
                raise TemplateNotFoundException(f"Template with id {template_id} not found")
            self.db.delete(model)
            self.db.commit()
        except TemplateNotFoundException:
            raise
        except Exception as e:
            self.db.rollback()
            raise RepositoryException(f"Error deleting template: {str(e)}")


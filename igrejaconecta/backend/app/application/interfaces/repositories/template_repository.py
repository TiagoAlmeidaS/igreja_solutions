"""
Template repository interface
"""

from abc import ABC, abstractmethod
from typing import Optional, List
from app.domain.entities.template import Template


class ITemplateRepository(ABC):
    """Interface for template repository"""
    
    @abstractmethod
    def create(self, template: Template) -> Template:
        """Create a new template"""
        pass
    
    @abstractmethod
    def get_by_id(self, template_id: int) -> Optional[Template]:
        """Get template by ID"""
        pass
    
    @abstractmethod
    def list_by_church(self, church_id: int, skip: int = 0, limit: int = 100) -> List[Template]:
        """List templates by church"""
        pass
    
    @abstractmethod
    def update(self, template: Template) -> Template:
        """Update template"""
        pass
    
    @abstractmethod
    def delete(self, template_id: int) -> None:
        """Delete template"""
        pass


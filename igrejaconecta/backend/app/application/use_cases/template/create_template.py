"""
Use case: Create Template
UC18: Criar Template (RF08)
"""

from datetime import datetime
from app.domain.entities.template import Template
from app.application.interfaces.repositories.template_repository import ITemplateRepository
from app.application.interfaces.repositories.church_repository import IChurchRepository
from app.application.dto.template_dto import TemplateCreateDTO, TemplateResponseDTO
from app.core.exceptions import ChurchNotFoundException


class CreateTemplateUseCase:
    """Use case for creating a template"""
    
    def __init__(
        self,
        template_repository: ITemplateRepository,
        church_repository: IChurchRepository
    ):
        self.template_repository = template_repository
        self.church_repository = church_repository
    
    def execute(self, church_id: int, dto: TemplateCreateDTO) -> TemplateResponseDTO:
        """Execute the use case"""
        # Verify church exists
        church = self.church_repository.get_by_id(church_id)
        if not church:
            raise ChurchNotFoundException(f"Church with id {church_id} not found")
        
        # Create domain entity
        template = Template(
            id=None,
            church_id=church_id,
            name=dto.name,
            message=dto.message,
            link_url=dto.link_url,
            button_text=dto.button_text,
            created_at=datetime.utcnow()
        )
        
        # Save to repository
        created_template = self.template_repository.create(template)
        
        # Convert to response DTO
        return TemplateResponseDTO(
            id=created_template.id,
            church_id=created_template.church_id,
            name=created_template.name,
            message=created_template.message,
            link_url=created_template.link_url,
            button_text=created_template.button_text,
            created_at=created_template.created_at
        )


"""
Use case: Create Contact
UC05: Criar Contato
"""

from datetime import datetime
from app.domain.entities.contact import Contact
from app.domain.value_objects.phone import Phone
from app.application.interfaces.repositories.contact_repository import IContactRepository
from app.application.interfaces.repositories.church_repository import IChurchRepository
from app.application.dto.contact_dto import ContactCreateDTO, ContactResponseDTO
from app.core.exceptions import ChurchNotFoundException, RepositoryException


class CreateContactUseCase:
    """Use case for creating a contact"""
    
    def __init__(
        self,
        contact_repository: IContactRepository,
        church_repository: IChurchRepository
    ):
        self.contact_repository = contact_repository
        self.church_repository = church_repository
    
    def execute(self, church_id: int, dto: ContactCreateDTO) -> ContactResponseDTO:
        """Execute the use case"""
        # Verify church exists
        church = self.church_repository.get_by_id(church_id)
        if not church:
            raise ChurchNotFoundException(f"Church with id {church_id} not found")
        
        # Check if contact with phone already exists
        phone_obj = Phone(dto.phone)
        existing = self.contact_repository.get_by_phone(church_id, phone_obj.value)
        if existing:
            raise RepositoryException(f"Contact with phone {dto.phone} already exists")
        
        # Create domain entity
        contact = Contact(
            id=None,
            church_id=church_id,
            name=dto.name,
            phone=phone_obj,
            tags=dto.tags or [],
            created_at=datetime.utcnow()
        )
        
        # Save to repository
        created_contact = self.contact_repository.create(contact)
        
        # Convert to response DTO
        return ContactResponseDTO(
            id=created_contact.id,
            church_id=created_contact.church_id,
            name=created_contact.name,
            phone=created_contact.phone.value,
            tags=created_contact.tags,
            created_at=created_contact.created_at
        )


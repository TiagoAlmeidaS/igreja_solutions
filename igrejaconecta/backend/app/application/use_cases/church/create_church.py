"""
Use case: Create Church
UC01: Criar Igreja (RF01)
"""

from datetime import datetime
from app.domain.entities.church import Church
from app.application.interfaces.repositories.church_repository import IChurchRepository
from app.application.dto.church_dto import ChurchCreateDTO, ChurchResponseDTO
from app.core.exceptions import RepositoryException


class CreateChurchUseCase:
    """Use case for creating a church"""
    
    def __init__(self, church_repository: IChurchRepository):
        self.church_repository = church_repository
    
    def execute(self, dto: ChurchCreateDTO) -> ChurchResponseDTO:
        """Execute the use case"""
        # Check if church with email already exists
        existing = self.church_repository.get_by_email(dto.email)
        if existing:
            raise RepositoryException(f"Church with email {dto.email} already exists")
        
        # Check if church with Firebase UID already exists
        existing_uid = self.church_repository.get_by_firebase_uid(dto.firebase_uid)
        if existing_uid:
            raise RepositoryException(f"Church with Firebase UID {dto.firebase_uid} already exists")
        
        # Create domain entity
        church = Church(
            id=None,
            name=dto.name,
            admin_name=dto.admin_name,
            email=dto.email,
            phone=dto.phone,
            whatsapp_phone_id=None,
            whatsapp_access_token=None,
            created_at=datetime.utcnow(),
            is_active=True
        )
        
        # Save to repository
        created_church = self.church_repository.create(church)
        
        # Update firebase_uid after creation (since it's not in domain entity)
        # This is a workaround - in a real scenario, you'd add firebase_uid to the domain entity
        from app.infrastructure.database.models.church_model import ChurchModel
        from app.infrastructure.database.database import SessionLocal
        db = SessionLocal()
        try:
            model = db.query(ChurchModel).filter(ChurchModel.id == created_church.id).first()
            if model:
                model.firebase_uid = dto.firebase_uid
                db.commit()
        finally:
            db.close()
        
        # Convert to response DTO
        return ChurchResponseDTO(
            id=created_church.id,
            name=created_church.name,
            admin_name=created_church.admin_name,
            email=created_church.email,
            phone=created_church.phone,
            whatsapp_phone_id=created_church.whatsapp_phone_id,
            is_active=created_church.is_active,
            created_at=created_church.created_at
        )


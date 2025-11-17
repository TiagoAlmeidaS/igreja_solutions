"""
Church API endpoints
"""

from fastapi import APIRouter, Depends, HTTPException
from typing import List
from sqlalchemy.orm import Session
from app.application.dto.church_dto import (
    ChurchCreateDTO,
    ChurchUpdateDTO,
    ChurchResponseDTO,
    WhatsAppConfigDTO,
    WhatsAppConfigResponseDTO,
)
from app.application.use_cases.church.create_church import CreateChurchUseCase
from app.application.use_cases.church.configure_whatsapp import ConfigureWhatsAppUseCase
from app.application.interfaces.repositories.church_repository import IChurchRepository
from app.application.interfaces.services.whatsapp_service import IWhatsAppService
from app.application.interfaces.services.firebase_service import IFirebaseService
from app.presentation.middleware.auth_middleware import get_firebase_uid
from app.core.dependencies import (
    get_db,
    get_church_repository,
    get_whatsapp_service,
    get_firebase_service,
)
from app.core.exceptions import ChurchNotFoundException

router = APIRouter()


@router.post("/", response_model=ChurchResponseDTO, status_code=201)
async def create_church(
    dto: ChurchCreateDTO,
    firebase_uid: str = Depends(get_firebase_uid),
    db: Session = Depends(get_db),
    firebase_service: IFirebaseService = Depends(get_firebase_service),
):
    """Create a new church"""
    # Verify Firebase UID matches
    if firebase_uid != dto.firebase_uid:
        raise HTTPException(status_code=403, detail="Firebase UID mismatch")
    
    church_repository = get_church_repository(db)
    use_case = CreateChurchUseCase(church_repository)
    return use_case.execute(dto)


@router.get("/me", response_model=ChurchResponseDTO)
async def get_my_church(
    firebase_uid: str = Depends(get_firebase_uid),
    db: Session = Depends(get_db),
):
    """Get current user's church"""
    church_repository = get_church_repository(db)
    church = church_repository.get_by_firebase_uid(firebase_uid)
    if not church:
        raise ChurchNotFoundException("Church not found")
    return ChurchResponseDTO(
        id=church.id,
        name=church.name,
        admin_name=church.admin_name,
        email=church.email,
        phone=church.phone,
        whatsapp_phone_id=church.whatsapp_phone_id,
        is_active=church.is_active,
        created_at=church.created_at
    )


@router.put("/me", response_model=ChurchResponseDTO)
async def update_my_church(
    dto: ChurchUpdateDTO,
    firebase_uid: str = Depends(get_firebase_uid),
    db: Session = Depends(get_db),
):
    """Update current user's church"""
    church_repository = get_church_repository(db)
    church = church_repository.get_by_firebase_uid(firebase_uid)
    if not church:
        raise ChurchNotFoundException("Church not found")
    
    if dto.name:
        church.name = dto.name
    if dto.admin_name is not None:
        church.admin_name = dto.admin_name
    if dto.phone:
        church.phone = dto.phone
    
    updated = church_repository.update(church)
    return ChurchResponseDTO(
        id=updated.id,
        name=updated.name,
        admin_name=updated.admin_name,
        email=updated.email,
        phone=updated.phone,
        whatsapp_phone_id=updated.whatsapp_phone_id,
        is_active=updated.is_active,
        created_at=updated.created_at
    )


@router.post("/whatsapp/config", response_model=WhatsAppConfigResponseDTO)
async def configure_whatsapp(
    dto: WhatsAppConfigDTO,
    firebase_uid: str = Depends(get_firebase_uid),
    db: Session = Depends(get_db),
    whatsapp_service: IWhatsAppService = Depends(get_whatsapp_service),
):
    """Configure WhatsApp Business for church"""
    church_repository = get_church_repository(db)
    church = church_repository.get_by_firebase_uid(firebase_uid)
    if not church:
        raise ChurchNotFoundException("Church not found")
    
    use_case = ConfigureWhatsAppUseCase(church_repository, whatsapp_service)
    return use_case.execute(church.id, dto)


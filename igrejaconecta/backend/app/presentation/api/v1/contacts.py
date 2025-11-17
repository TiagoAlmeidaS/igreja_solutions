"""
Contacts API endpoints
"""

from fastapi import APIRouter, Depends, UploadFile, File
from typing import List
from sqlalchemy.orm import Session
from app.application.dto.contact_dto import (
    ContactCreateDTO,
    ContactUpdateDTO,
    ContactResponseDTO,
    ContactBulkCreateDTO,
)
from app.application.use_cases.contact.create_contact import CreateContactUseCase
from app.application.use_cases.contact.import_contacts_csv import ImportContactsCSVUseCase
from app.application.interfaces.repositories.contact_repository import IContactRepository
from app.application.interfaces.repositories.church_repository import IChurchRepository
from app.presentation.middleware.auth_middleware import get_firebase_uid
from app.core.dependencies import (
    get_db,
    get_contact_repository,
    get_church_repository,
)
from app.core.exceptions import ChurchNotFoundException

router = APIRouter()


def get_current_church_id(
    firebase_uid: str = Depends(get_firebase_uid),
    db: Session = Depends(get_db),
) -> int:
    """Get current church ID from Firebase UID"""
    church_repository = get_church_repository(db)
    church = church_repository.get_by_firebase_uid(firebase_uid)
    if not church:
        raise ChurchNotFoundException("Church not found")
    return church.id


@router.post("/", response_model=ContactResponseDTO, status_code=201)
async def create_contact(
    dto: ContactCreateDTO,
    church_id: int = Depends(get_current_church_id),
    db: Session = Depends(get_db),
):
    """Create a new contact"""
    contact_repository = get_contact_repository(db)
    church_repository = get_church_repository(db)
    use_case = CreateContactUseCase(contact_repository, church_repository)
    return use_case.execute(church_id, dto)


@router.get("/", response_model=List[ContactResponseDTO])
async def list_contacts(
    skip: int = 0,
    limit: int = 100,
    church_id: int = Depends(get_current_church_id),
    db: Session = Depends(get_db),
):
    """List contacts"""
    contact_repository = get_contact_repository(db)
    contacts = contact_repository.list_by_church(church_id, skip=skip, limit=limit)
    return [
        ContactResponseDTO(
            id=c.id,
            church_id=c.church_id,
            name=c.name,
            phone=c.phone.value,
            tags=c.tags,
            created_at=c.created_at
        )
        for c in contacts
    ]


@router.post("/upload", response_model=List[ContactResponseDTO])
async def upload_contacts_csv(
    file: UploadFile = File(...),
    church_id: int = Depends(get_current_church_id),
    db: Session = Depends(get_db),
):
    """Import contacts from CSV file"""
    content = await file.read()
    csv_content = content.decode('utf-8')
    
    contact_repository = get_contact_repository(db)
    church_repository = get_church_repository(db)
    use_case = ImportContactsCSVUseCase(contact_repository, church_repository)
    return use_case.execute(church_id, csv_content)


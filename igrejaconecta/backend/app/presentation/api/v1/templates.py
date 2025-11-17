"""
Templates API endpoints
"""

from fastapi import APIRouter, Depends
from typing import List
from sqlalchemy.orm import Session
from app.application.dto.template_dto import (
    TemplateCreateDTO,
    TemplateResponseDTO,
)
from app.application.use_cases.template.create_template import CreateTemplateUseCase
from app.application.interfaces.repositories.template_repository import ITemplateRepository
from app.application.interfaces.repositories.church_repository import IChurchRepository
from app.presentation.middleware.auth_middleware import get_firebase_uid
from app.core.dependencies import (
    get_db,
    get_template_repository,
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


@router.post("/", response_model=TemplateResponseDTO, status_code=201)
async def create_template(
    dto: TemplateCreateDTO,
    church_id: int = Depends(get_current_church_id),
    db: Session = Depends(get_db),
):
    """Create a new template"""
    template_repository = get_template_repository(db)
    church_repository = get_church_repository(db)
    use_case = CreateTemplateUseCase(template_repository, church_repository)
    return use_case.execute(church_id, dto)


@router.get("/", response_model=List[TemplateResponseDTO])
async def list_templates(
    skip: int = 0,
    limit: int = 100,
    church_id: int = Depends(get_current_church_id),
    db: Session = Depends(get_db),
):
    """List templates"""
    template_repository = get_template_repository(db)
    templates = template_repository.list_by_church(church_id, skip=skip, limit=limit)
    return [
        TemplateResponseDTO(
            id=t.id,
            church_id=t.church_id,
            name=t.name,
            message=t.message,
            link_url=t.link_url,
            button_text=t.button_text,
            created_at=t.created_at
        )
        for t in templates
    ]


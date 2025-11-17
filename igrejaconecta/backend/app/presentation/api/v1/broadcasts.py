"""
Broadcasts API endpoints
"""

from fastapi import APIRouter, Depends
from typing import List, Optional
from sqlalchemy.orm import Session
from app.application.dto.broadcast_dto import (
    BroadcastCreateDTO,
    BroadcastResponseDTO,
    BroadcastStatisticsDTO,
)
from app.application.use_cases.broadcast.create_broadcast import CreateBroadcastUseCase
from app.application.use_cases.broadcast.send_broadcast import SendBroadcastUseCase
from app.application.interfaces.repositories.broadcast_repository import IBroadcastRepository
from app.application.interfaces.repositories.church_repository import IChurchRepository
from app.application.interfaces.services.whatsapp_service import IWhatsAppService
from app.presentation.middleware.auth_middleware import get_firebase_uid
from app.core.dependencies import (
    get_db,
    get_broadcast_repository,
    get_church_repository,
    get_whatsapp_service,
    get_contact_repository,
)
from app.core.exceptions import ChurchNotFoundException
from app.domain.entities.broadcast import BroadcastStatus

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


@router.post("/", response_model=BroadcastResponseDTO, status_code=201)
async def create_broadcast(
    dto: BroadcastCreateDTO,
    church_id: int = Depends(get_current_church_id),
    db: Session = Depends(get_db),
):
    """Create a new broadcast"""
    broadcast_repository = get_broadcast_repository(db)
    church_repository = get_church_repository(db)
    use_case = CreateBroadcastUseCase(broadcast_repository, church_repository)
    return use_case.execute(church_id, dto)


@router.get("/", response_model=List[BroadcastResponseDTO])
async def list_broadcasts(
    skip: int = 0,
    limit: int = 100,
    status: Optional[str] = None,
    church_id: int = Depends(get_current_church_id),
    db: Session = Depends(get_db),
):
    """List broadcasts"""
    broadcast_repository = get_broadcast_repository(db)
    broadcast_status = BroadcastStatus(status) if status else None
    broadcasts = broadcast_repository.list_by_church(
        church_id,
        skip=skip,
        limit=limit,
        status=broadcast_status
    )
    return [
        BroadcastResponseDTO(
            id=b.id,
            church_id=b.church_id,
            title=b.title,
            message=b.message,
            link_url=b.link_url,
            button_text=b.button_text,
            contact_tags=b.contact_tags,
            scheduled_at=b.scheduled_at,
            sent_at=b.sent_at,
            status=b.status.value,
            total_sent=b.total_sent,
            created_at=b.created_at
        )
        for b in broadcasts
    ]


@router.post("/{broadcast_id}/send")
async def send_broadcast(
    broadcast_id: int,
    church_id: int = Depends(get_current_church_id),
    db: Session = Depends(get_db),
    whatsapp_service: IWhatsAppService = Depends(get_whatsapp_service),
):
    """Send broadcast immediately"""
    broadcast_repository = get_broadcast_repository(db)
    church_repository = get_church_repository(db)
    contact_repository = get_contact_repository(db)
    use_case = SendBroadcastUseCase(
        broadcast_repository,
        contact_repository,
        church_repository,
        whatsapp_service
    )
    return use_case.execute(church_id, broadcast_id)


@router.get("/statistics", response_model=BroadcastStatisticsDTO)
async def get_statistics(
    church_id: int = Depends(get_current_church_id),
    db: Session = Depends(get_db),
):
    """Get broadcast statistics"""
    broadcast_repository = get_broadcast_repository(db)
    stats = broadcast_repository.get_statistics(church_id)
    return BroadcastStatisticsDTO(**stats)


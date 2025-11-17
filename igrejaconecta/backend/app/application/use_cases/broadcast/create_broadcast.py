"""
Use case: Create Broadcast
UC12: Criar Transmissão (RF04)
"""

from datetime import datetime
from app.domain.entities.broadcast import Broadcast, BroadcastStatus
from app.application.interfaces.repositories.broadcast_repository import IBroadcastRepository
from app.application.interfaces.repositories.church_repository import IChurchRepository
from app.application.dto.broadcast_dto import BroadcastCreateDTO, BroadcastResponseDTO
from app.core.exceptions import ChurchNotFoundException


class CreateBroadcastUseCase:
    """Use case for creating a broadcast"""
    
    def __init__(
        self,
        broadcast_repository: IBroadcastRepository,
        church_repository: IChurchRepository
    ):
        self.broadcast_repository = broadcast_repository
        self.church_repository = church_repository
    
    def execute(self, church_id: int, dto: BroadcastCreateDTO) -> BroadcastResponseDTO:
        """Execute the use case"""
        # Verify church exists
        church = self.church_repository.get_by_id(church_id)
        if not church:
            raise ChurchNotFoundException(f"Church with id {church_id} not found")
        
        # Determine status
        status = BroadcastStatus.PENDING
        if dto.scheduled_at:
            if dto.scheduled_at <= datetime.utcnow():
                raise ValueError("Scheduled time must be in the future")
        
        # Create domain entity
        broadcast = Broadcast(
            id=None,
            church_id=church_id,
            title=dto.title,
            message=dto.message,
            link_url=dto.link_url,
            button_text=dto.button_text,
            contact_tags=dto.contact_tags or [],
            scheduled_at=dto.scheduled_at,
            sent_at=None,
            status=status,
            total_sent=0,
            created_at=datetime.utcnow()
        )
        
        if dto.scheduled_at:
            broadcast.schedule(dto.scheduled_at)
        
        # Save to repository
        created_broadcast = self.broadcast_repository.create(broadcast)
        
        # Convert to response DTO
        return BroadcastResponseDTO(
            id=created_broadcast.id,
            church_id=created_broadcast.church_id,
            title=created_broadcast.title,
            message=created_broadcast.message,
            link_url=created_broadcast.link_url,
            button_text=created_broadcast.button_text,
            contact_tags=created_broadcast.contact_tags,
            scheduled_at=created_broadcast.scheduled_at,
            sent_at=created_broadcast.sent_at,
            status=created_broadcast.status.value,
            total_sent=created_broadcast.total_sent,
            created_at=created_broadcast.created_at
        )


"""
Use case: Send Broadcast
UC13: Enviar Transmissão Imediata (RF06)
"""

from app.domain.entities.broadcast import BroadcastStatus
from app.application.interfaces.repositories.broadcast_repository import IBroadcastRepository
from app.application.interfaces.repositories.contact_repository import IContactRepository
from app.application.interfaces.repositories.church_repository import IChurchRepository
from app.application.interfaces.services.whatsapp_service import IWhatsAppService
from app.core.exceptions import (
    BroadcastNotFoundException,
    ChurchNotFoundException,
    WhatsAppConfigurationException
)


class SendBroadcastUseCase:
    """Use case for sending a broadcast immediately"""
    
    def __init__(
        self,
        broadcast_repository: IBroadcastRepository,
        contact_repository: IContactRepository,
        church_repository: IChurchRepository,
        whatsapp_service: IWhatsAppService
    ):
        self.broadcast_repository = broadcast_repository
        self.contact_repository = contact_repository
        self.church_repository = church_repository
        self.whatsapp_service = whatsapp_service
    
    def execute(self, church_id: int, broadcast_id: int) -> dict:
        """Execute the use case"""
        # Get broadcast
        broadcast = self.broadcast_repository.get_by_id(broadcast_id)
        if not broadcast:
            raise BroadcastNotFoundException(f"Broadcast with id {broadcast_id} not found")
        
        # Verify ownership
        if broadcast.church_id != church_id:
            raise BroadcastNotFoundException("Broadcast not found")
        
        # Check if can be sent
        if not broadcast.can_be_sent():
            raise ValueError(f"Cannot send broadcast with status {broadcast.status.value}")
        
        # Get church
        church = self.church_repository.get_by_id(church_id)
        if not church:
            raise ChurchNotFoundException(f"Church with id {church_id} not found")
        
        # Check WhatsApp configuration
        if not church.is_whatsapp_configured():
            raise WhatsAppConfigurationException("WhatsApp not configured for this church")
        
        # Get contacts by tags
        contacts = []
        if broadcast.contact_tags:
            contacts = self.contact_repository.list_by_tags(church_id, broadcast.contact_tags)
        else:
            contacts = self.contact_repository.list_by_church(church_id, limit=10000)
        
        # Send messages
        success_count = 0
        failed_count = 0
        
        for contact in contacts:
            try:
                if broadcast.link_url and broadcast.button_text:
                    # Send interactive message
                    self.whatsapp_service.send_interactive_message(
                        to=contact.phone.value,
                        body=broadcast.message,
                        button_text=broadcast.button_text,
                        url=broadcast.link_url,
                        phone_id=church.whatsapp_phone_id,
                        token=church.whatsapp_access_token
                    )
                else:
                    # Send text message
                    self.whatsapp_service.send_text_message(
                        to=contact.phone.value,
                        message=broadcast.message,
                        phone_id=church.whatsapp_phone_id,
                        token=church.whatsapp_access_token
                    )
                success_count += 1
            except Exception:
                failed_count += 1
        
        # Update broadcast
        broadcast.update_total_sent(success_count)
        broadcast.send()
        self.broadcast_repository.update(broadcast)
        
        return {
            "success": success_count,
            "failed": failed_count,
            "total": len(contacts)
        }


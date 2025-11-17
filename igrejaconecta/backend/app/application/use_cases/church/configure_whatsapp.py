"""
Use case: Configure WhatsApp Business
UC03: Configurar WhatsApp Business (RF02)
"""

from app.domain.entities.church import Church
from app.application.interfaces.repositories.church_repository import IChurchRepository
from app.application.interfaces.services.whatsapp_service import IWhatsAppService
from app.application.dto.church_dto import WhatsAppConfigDTO, WhatsAppConfigResponseDTO
from app.core.exceptions import ChurchNotFoundException, WhatsAppConfigurationException


class ConfigureWhatsAppUseCase:
    """Use case for configuring WhatsApp Business"""
    
    def __init__(
        self,
        church_repository: IChurchRepository,
        whatsapp_service: IWhatsAppService
    ):
        self.church_repository = church_repository
        self.whatsapp_service = whatsapp_service
    
    def execute(self, church_id: int, dto: WhatsAppConfigDTO) -> WhatsAppConfigResponseDTO:
        """Execute the use case"""
        # Get church
        church = self.church_repository.get_by_id(church_id)
        if not church:
            raise ChurchNotFoundException(f"Church with id {church_id} not found")
        
        # Validate credentials
        if not self.whatsapp_service.validate_credentials(dto.phone_id, dto.access_token):
            raise WhatsAppConfigurationException("Invalid WhatsApp credentials")
        
        # Configure WhatsApp
        church.configure_whatsapp(dto.phone_id, dto.access_token)
        
        # Update church
        updated_church = self.church_repository.update(church)
        
        return WhatsAppConfigResponseDTO(
            phone_id=updated_church.whatsapp_phone_id,
            is_configured=updated_church.is_whatsapp_configured()
        )


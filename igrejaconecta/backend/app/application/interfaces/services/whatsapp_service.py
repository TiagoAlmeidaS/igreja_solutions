"""
WhatsApp service interface
"""

from abc import ABC, abstractmethod
from typing import List, Dict, Any


class IWhatsAppService(ABC):
    """Interface for WhatsApp service"""
    
    @abstractmethod
    def send_interactive_message(
        self,
        to: str,
        body: str,
        button_text: str,
        url: str,
        phone_id: str,
        token: str
    ) -> Dict[str, Any]:
        """Send interactive message with button"""
        pass
    
    @abstractmethod
    def send_text_message(
        self,
        to: str,
        message: str,
        phone_id: str,
        token: str
    ) -> Dict[str, Any]:
        """Send simple text message"""
        pass
    
    @abstractmethod
    def send_bulk_messages(
        self,
        recipients: List[str],
        message: str,
        phone_id: str,
        token: str
    ) -> Dict[str, Any]:
        """Send bulk messages"""
        pass
    
    @abstractmethod
    def validate_credentials(self, phone_id: str, token: str) -> bool:
        """Validate WhatsApp credentials"""
        pass


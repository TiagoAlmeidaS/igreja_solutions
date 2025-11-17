"""
WhatsApp Cloud API client
"""

import requests
from typing import List, Dict, Any
from app.application.interfaces.services.whatsapp_service import IWhatsAppService
from app.core.config import settings
from app.core.exceptions import WhatsAppConfigurationException


class WhatsAppClient(IWhatsAppService):
    """WhatsApp Cloud API service implementation"""
    
    def __init__(self):
        self.api_version = settings.WHATSAPP_API_VERSION
        self.base_url = f"https://graph.facebook.com/{self.api_version}"
    
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
        headers = {
            "Authorization": f"Bearer {token}",
            "Content-Type": "application/json"
        }
        
        payload = {
            "messaging_product": "whatsapp",
            "to": to,
            "type": "interactive",
            "interactive": {
                "type": "button",
                "body": {"text": body},
                "action": {
                    "buttons": [
                        {
                            "type": "url",
                            "url": url,
                            "title": button_text[:20]  # Max 20 characters
                        }
                    ]
                }
            }
        }
        
        api_url = f"{self.base_url}/{phone_id}/messages"
        
        try:
            response = requests.post(api_url, json=payload, headers=headers, timeout=30)
            response.raise_for_status()
            return response.json()
        except requests.exceptions.RequestException as e:
            raise WhatsAppConfigurationException(f"WhatsApp API error: {str(e)}")
    
    def send_text_message(
        self,
        to: str,
        message: str,
        phone_id: str,
        token: str
    ) -> Dict[str, Any]:
        """Send simple text message"""
        headers = {
            "Authorization": f"Bearer {token}",
            "Content-Type": "application/json"
        }
        
        payload = {
            "messaging_product": "whatsapp",
            "to": to,
            "type": "text",
            "text": {"body": message}
        }
        
        api_url = f"{self.base_url}/{phone_id}/messages"
        
        try:
            response = requests.post(api_url, json=payload, headers=headers, timeout=30)
            response.raise_for_status()
            return response.json()
        except requests.exceptions.RequestException as e:
            raise WhatsAppConfigurationException(f"WhatsApp API error: {str(e)}")
    
    def send_bulk_messages(
        self,
        recipients: List[str],
        message: str,
        phone_id: str,
        token: str
    ) -> Dict[str, Any]:
        """Send bulk messages"""
        results = {
            "success": [],
            "failed": []
        }
        
        for recipient in recipients:
            try:
                result = self.send_text_message(recipient, message, phone_id, token)
                results["success"].append({"to": recipient, "result": result})
            except Exception as e:
                results["failed"].append({"to": recipient, "error": str(e)})
        
        return results
    
    def validate_credentials(self, phone_id: str, token: str) -> bool:
        """Validate WhatsApp credentials by sending a test message"""
        # Try to get phone number info as validation
        headers = {
            "Authorization": f"Bearer {token}",
        }
        
        api_url = f"{self.base_url}/{phone_id}"
        
        try:
            response = requests.get(api_url, headers=headers, timeout=10)
            return response.status_code == 200
        except requests.exceptions.RequestException:
            return False


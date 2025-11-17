"""
WhatsApp Cloud API service
"""

import requests
from fastapi import HTTPException
from typing import Optional


def send_interactive_message(
    to: str,
    body: str,
    button_text: str,
    url: str,
    phone_id: str,
    token: str
) -> dict:
    """
    Send an interactive message with button via WhatsApp Cloud API
    
    Args:
        to: Recipient phone number (with country code, no +)
        body: Message body text
        button_text: Text for the button (max 20 chars)
        url: URL to open when button is clicked
        phone_id: WhatsApp Phone Number ID
        token: WhatsApp Access Token
        
    Returns:
        API response dict
    """
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
    
    api_url = f"https://graph.facebook.com/v20.0/{phone_id}/messages"
    
    try:
        response = requests.post(api_url, json=payload, headers=headers)
        response.raise_for_status()
        return response.json()
    except requests.exceptions.RequestException as e:
        raise HTTPException(
            status_code=500,
            detail=f"WhatsApp API error: {str(e)}"
        )


def send_text_message(
    to: str,
    message: str,
    phone_id: str,
    token: str
) -> dict:
    """
    Send a simple text message via WhatsApp Cloud API
    
    Args:
        to: Recipient phone number
        message: Message text
        phone_id: WhatsApp Phone Number ID
        token: WhatsApp Access Token
        
    Returns:
        API response dict
    """
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
    
    api_url = f"https://graph.facebook.com/v20.0/{phone_id}/messages"
    
    try:
        response = requests.post(api_url, json=payload, headers=headers)
        response.raise_for_status()
        return response.json()
    except requests.exceptions.RequestException as e:
        raise HTTPException(
            status_code=500,
            detail=f"WhatsApp API error: {str(e)}"
        )


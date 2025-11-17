"""
Phone value object
"""

import re
from typing import Optional
from app.core.exceptions import InvalidPhoneNumberException


class Phone:
    """Phone number value object"""
    
    def __init__(self, value: str):
        """Initialize phone with validation"""
        self._value = self._validate_and_normalize(value)
    
    def _validate_and_normalize(self, phone: str) -> str:
        """Validate and normalize phone number"""
        if not phone:
            raise InvalidPhoneNumberException("Phone number cannot be empty")
        
        # Remove all non-digit characters
        digits_only = re.sub(r'\D', '', phone)
        
        # Basic validation: should have 10-15 digits
        if len(digits_only) < 10 or len(digits_only) > 15:
            raise InvalidPhoneNumberException(
                f"Phone number must have between 10 and 15 digits, got {len(digits_only)}"
            )
        
        return digits_only
    
    @property
    def value(self) -> str:
        """Get phone value"""
        return self._value
    
    def __eq__(self, other) -> bool:
        """Compare phone numbers"""
        if not isinstance(other, Phone):
            return False
        return self._value == other._value
    
    def __hash__(self) -> int:
        """Hash phone number"""
        return hash(self._value)
    
    def __str__(self) -> str:
        """String representation"""
        return self._value
    
    def __repr__(self) -> str:
        """Representation"""
        return f"Phone('{self._value}')"


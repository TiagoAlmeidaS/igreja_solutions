"""
Supabase client
"""

from typing import Optional
from supabase import create_client, Client
from app.core.config import settings


class SupabaseClient:
    """Supabase client wrapper"""
    
    def __init__(self):
        """Initialize Supabase client"""
        if settings.SUPABASE_URL and settings.SUPABASE_KEY:
            self.client: Optional[Client] = create_client(
                settings.SUPABASE_URL,
                settings.SUPABASE_KEY
            )
        else:
            self.client = None
    
    def get_client(self) -> Optional[Client]:
        """Get Supabase client instance"""
        return self.client
    
    def is_configured(self) -> bool:
        """Check if Supabase is configured"""
        return self.client is not None


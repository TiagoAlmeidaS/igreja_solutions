"""
Use case: Import Contacts via CSV
UC06: Importar Contatos via CSV (RF03)
"""

import pandas as pd
from io import StringIO
from typing import List
from datetime import datetime
from app.domain.entities.contact import Contact
from app.domain.value_objects.phone import Phone
from app.application.interfaces.repositories.contact_repository import IContactRepository
from app.application.interfaces.repositories.church_repository import IChurchRepository
from app.application.dto.contact_dto import ContactResponseDTO
from app.core.exceptions import ChurchNotFoundException, RepositoryException


class ImportContactsCSVUseCase:
    """Use case for importing contacts from CSV"""
    
    def __init__(
        self,
        contact_repository: IContactRepository,
        church_repository: IChurchRepository
    ):
        self.contact_repository = contact_repository
        self.church_repository = church_repository
    
    def execute(self, church_id: int, csv_content: str) -> List[ContactResponseDTO]:
        """Execute the use case"""
        # Verify church exists
        church = self.church_repository.get_by_id(church_id)
        if not church:
            raise ChurchNotFoundException(f"Church with id {church_id} not found")
        
        try:
            # Parse CSV
            df = pd.read_csv(StringIO(csv_content))
            
            # Validate required columns
            required_columns = ['phone']
            if not all(col in df.columns for col in required_columns):
                raise RepositoryException(f"CSV must contain columns: {required_columns}")
            
            # Create contacts
            contacts = []
            for _, row in df.iterrows():
                try:
                    phone = Phone(str(row['phone']))
                    name = row.get('name', None)
                    tags = row.get('tags', '').split(',') if 'tags' in row else []
                    tags = [tag.strip() for tag in tags if tag.strip()]
                    
                    # Check if contact already exists
                    existing = self.contact_repository.get_by_phone(church_id, phone.value)
                    if existing:
                        continue  # Skip duplicates
                    
                    contact = Contact(
                        id=None,
                        church_id=church_id,
                        name=name,
                        phone=phone,
                        tags=tags,
                        created_at=datetime.utcnow()
                    )
                    contacts.append(contact)
                except Exception as e:
                    # Skip invalid rows
                    continue
            
            # Bulk create
            if contacts:
                created_contacts = self.contact_repository.bulk_create(contacts)
                return [
                    ContactResponseDTO(
                        id=c.id,
                        church_id=c.church_id,
                        name=c.name,
                        phone=c.phone.value,
                        tags=c.tags,
                        created_at=c.created_at
                    )
                    for c in created_contacts
                ]
            
            return []
        except Exception as e:
            raise RepositoryException(f"Error importing CSV: {str(e)}")


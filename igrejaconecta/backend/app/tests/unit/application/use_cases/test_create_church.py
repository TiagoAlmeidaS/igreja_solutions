"""
Unit tests for CreateChurchUseCase
"""

import pytest
from unittest.mock import Mock, patch, MagicMock
from datetime import datetime
from app.application.use_cases.church.create_church import CreateChurchUseCase
from app.application.dto.church_dto import ChurchCreateDTO
from app.core.exceptions import RepositoryException
from app.domain.entities.church import Church


class MockChurchRepository:
    """Mock church repository for testing"""
    
    def __init__(self):
        self.churches = []
        self.firebase_uids = {}
    
    def create(self, church):
        church.id = len(self.churches) + 1
        self.churches.append(church)
        return church
    
    def get_by_email(self, email):
        for church in self.churches:
            if church.email == email:
                return church
        return None
    
    def get_by_firebase_uid(self, firebase_uid):
        return self.firebase_uids.get(firebase_uid)


@patch('app.infrastructure.database.database.SessionLocal')
def test_create_church_success(mock_session_local):
    """Test successful church creation"""
    repo = MockChurchRepository()
    use_case = CreateChurchUseCase(repo)
    
    dto = ChurchCreateDTO(
        name="Igreja Teste",
        email="teste@igreja.com",
        firebase_uid="firebase_uid_123"
    )
    
    # Mock database operations for firebase_uid update
    mock_db = MagicMock()
    mock_db_instance = MagicMock()
    mock_db_instance.id = 1
    mock_db.query.return_value.filter.return_value.first.return_value = mock_db_instance
    mock_session_local.return_value = mock_db
    
    result = use_case.execute(dto)
    
    assert result.id is not None
    assert result.name == "Igreja Teste"
    assert result.email == "teste@igreja.com"
    assert result.is_active is True


@patch('app.infrastructure.database.database.SessionLocal')
def test_create_church_duplicate_email(mock_session_local):
    """Test church creation with duplicate email"""
    repo = MockChurchRepository()
    use_case = CreateChurchUseCase(repo)
    
    # Mock database operations
    mock_db = MagicMock()
    mock_db_instance = MagicMock()
    mock_db_instance.id = 1
    mock_db.query.return_value.filter.return_value.first.return_value = mock_db_instance
    mock_session_local.return_value = mock_db
    
    # Create first church
    dto1 = ChurchCreateDTO(
        name="Igreja 1",
        email="teste@igreja.com",
        firebase_uid="firebase_uid_123"
    )
    use_case.execute(dto1)
    
    # Try to create duplicate
    dto2 = ChurchCreateDTO(
        name="Igreja 2",
        email="teste@igreja.com",
        firebase_uid="firebase_uid_456"
    )
    
    with pytest.raises(RepositoryException):
        use_case.execute(dto2)


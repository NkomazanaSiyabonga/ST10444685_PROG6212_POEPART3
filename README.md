# ST10444685_PROG6212_POEPART3

## Contract Monthly Claim System - Part 3 Enhancements
## Project Overview
A comprehensive claims management system for academic institutions with automated workflows, role-based access control, and database integration.

### Major Changes Made
##### 1. Database Integration & Entity Framework

- Added SQL Server Database with Entity Framework Core

- Replaced in-memory storage with proper database persistence

- Created database models: User, Claim, SupportingDocument

- Added migrations for database schema management

- Generated SQL script for database deployment

##### 2. Role Structure & Workflow Enhancement

- Updated role hierarchy: Lecturer → Coordinator → HR Manager

- Enhanced ClaimStatus enum with new workflow states:

- Submitted → Verified → Approved

- RejectedByCoordinator / RejectedByHR / ReturnedForCorrection

- Added tracking fields to Claim model:

- RejectionNotes, RejectedBy, VerifiedBy, ApprovedBy

- VerificationDate, ApprovalDate

##### 3. Automation Features

- Lecturer View Automation
Real-time auto-calculation of Total Amount (Hours × Rate)

- Enhanced validation with immediate feedback

- Auto-population of lecturer names from session

- File upload with type and size validation

- Coordinator View Automation
- Bulk claim processing with multi-select functionality

- Automated validation rules for policy compliance

- Smart rejection system with required notes

- HR-returned claims handling workflow

- HR Manager View Automation
- User management system for creating/updating users

- Automated report generation (PDF/Excel exports)

- Approval workflow automation with one-click actions

- Dashboard analytics with claim statistics

##### 4. Security & Authentication
- Session-based authentication with role validation

- Custom authorization attribute for role-based access control

- Password hashing using BCrypt

- Secure file upload with validation

##### 5. User Interface Improvements
- Role-based navigation with appropriate menu items

- Dashboard button accessible to all authenticated users

- Responsive design with Bootstrap 5

- Enhanced error handling and user feedback
  
### Workflow Process
#### Claim Submission Flow
- Lecturer submits claim → Auto-validation & calculation

- Coordinator verifies claim → Policy compliance checking

- HR Manager approves claim → Final validation & payment processing

#### Rejection Handling
- HR rejects → Returns to Coordinator with notes

- Coordinator rejects → Returns to Lecturer with explanation

- Claims cannot go directly from Lecturer to HR Manager

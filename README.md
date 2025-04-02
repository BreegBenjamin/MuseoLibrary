# MuseoLabrary
Description

This project allows users to upload images of historical monuments through a Flutter application. The image is analyzed using various Azure services to determine whether the monument is correctly identified. If the prediction is correct, a digital sticker is unlocked and stored in Cosmos DB and Blob Storage. Additionally, users can manage their own catalog of travel images.

Workflow

Image Upload: The user uploads an image from the Flutter app.

Storage in Blob Storage: The image is stored in an Azure Blob Storage container.

Analysis with Azure Vision: The image is analyzed to extract relevant monument information.

Prediction with DeepSeek (Azure AI Foundry): The analyzed data is sent to the AI service for monument verification.

Sticker Registration in Cosmos DB: If the prediction is valid, a sticker is unlocked and stored in the database along with the user information.

Image Management with Azure Function:

Deletes the image from the original Blob Storage container.

Copies the image to another private container for the user to manage their travel catalog.

Use of Service Bus: Azure Service Bus is used to communicate events between analysis and storage services.

Technologies and Services Used

Azure Blob Storage: Image storage.

Azure Vision Analyzer: Image analysis and data extraction.

Azure AI Foundry - DeepSeek: AI-based prediction of the monument.

Azure Cosmos DB: Database for storing user information and unlocked stickers.

Azure Service Bus: Communication between services.

Azure Functions: Automation of image deletion and management.

Installation and Setup

Clone this repository:

Configure Azure credentials in a .env file or appsettings.json.

Deploy resources in Azure using Terraform or manually via the Azure portal.

Build and run the backend in C#.

Run the Flutter application to test image uploads.

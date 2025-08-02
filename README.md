# Desafio Técnico - Sistema de Proposta de Crédito

Este projeto é a solução para o desafio técnico de um sistema de crédito bancário, implementado com uma arquitetura de microsserviços orientada a eventos.

## Arquitetura da Solução

A solução é baseada em uma arquitetura de microsserviços desacoplados que se comunicam através de um hub de mensageria (RabbitMQ). O fluxo de trabalho é o seguinte:

1.  **Microsserviço de Clientes (`Customers.Api`)**: Recebe a requisição via API REST para cadastrar um novo cliente. Após a validação, ele publica um evento (`ClientCreatedEvent`) no RabbitMQ.
2.  **Microsserviço de Proposta de Crédito (`CreditProposals.Api`)**: Atua como um consumidor, ouvindo o evento de cliente criado. Ele simula a análise de crédito e publica um novo evento (`CreditProposalCreatedEvent`) com o resultado.
3.  **Microsserviço de Cartão de Crédito (`CreditCards.Api`)**: Consome o evento de proposta de crédito. Se a proposta for aprovada, ele simula a emissão de um cartão e pode publicar um evento de confirmação.

## Tecnologias Utilizadas

* **.NET 8.0**: Framework para o desenvolvimento dos microsserviços.
* **RabbitMQ**: Hub de mensageria para a comunicação assíncrona.
* **Docker Compose**: Orquestração e gerenciamento dos contêineres dos microsserviços e do RabbitMQ.
* **Serilog**: Para logging estruturado, facilitando o monitoramento.
* **Clean Architecture**: O projeto está estruturado em camadas (`Contracts`, `Core`, `Messaging`) para separação de responsabilidades.

## Como Executar a Solução

As instruções abaixo permitem iniciar toda a solução com apenas um comando, graças ao Docker Compose.

### Pré-requisitos

* Docker Desktop instalado e em execução.

### Passos

1.  Clone este repositório para sua máquina local:
    ```bash
    git clone
    cd teste-credito-banco
    ```
2.  Construa as imagens e inicie todos os contêineres:
    ```bash
    docker compose up --build
    ```
Após a execução do comando, todos os três microsserviços e o RabbitMQ estarão rodando.

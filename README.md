# **Imóveis API**

API REST desenvolvida em **.NET 8** para **cadastro de imóveis** de proprietários, com fluxo em etapas, possibilidade de retomada e cálculo do valor total das propriedades.

---

## **Objetivo do Projeto**

O objetivo é implementar uma **API para gestão de cadastros de imóveis** que:

* Permita **cadastrar um proprietário e seus imóveis em etapas** (proprietário → imóveis → finalização).
* **Retome cadastros incompletos** sem perda de dados.
* **Liste e consulte cadastros** por documento do proprietário.
* **Calcule o valor total dos imóveis** com base em regras de preço por hectare.

---

## **Estrutura de Pastas**

```
ImoveisAPI/
│
├── Controllers/           # Endpoints da API
│   ├── CadastroController.cs
│   ├── ProprietarioController.cs
│
├── DTOs/                  # Objetos de transferência de dados (entrada e saída)
│   ├── ProprietarioDTO.cs
│   ├── ProprietarioDetalhadoDTO.cs
│   ├── ImovelDTO.cs
│   ├── CadastroResumoDTO.cs
│   ├── CadastroStatusDTO.cs
│
├── Middleware/            # Tratamento global de exceções
│   ├── ExceptionMiddleware.cs
│
├── Models/                # Entidades de domínio
│   ├── Proprietario.cs
│   ├── Imovel.cs
│   ├── Cadastro.cs
│
├── Repositories/          # Acesso ao banco (camada de persistência)
│   ├── CadastroRepository.cs
│   ├── ProprietarioRepository.cs
│
├── Services/              # Regras de negócio
│   ├── CadastroService.cs
│   ├── ProprietarioService.cs
│   ├── ValorImovelService.cs
│
├── Data/                  # Configuração do Entity Framework
│   ├── AppDbContext.cs
│
├── Program.cs             # Configuração da aplicação
├── appsettings.json       # Configuração do banco e app
```

---

## **Como Rodar (Passo a Passo)**

1. **Clonar o repositório**

```bash
git clone https://github.com/seu-usuario/imoveis-api.git
cd imoveis-api
```

2. **Instalar dependências**

```bash
dotnet restore
```

3. **Gerar o banco de dados** (usando EF Core com SQLite)

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

4. **Executar a aplicação**

```bash
dotnet run
```

5. **Acessar no navegador**

* Swagger UI: `https://localhost:5001/swagger`
* API Base: `https://localhost:5001/api`

---

## **Exemplos de Requisições**

### **1. Iniciar cadastro**

`POST /api/cadastro/iniciar`

```json
{
  "nome": "João da Silva",
  "documento": "12345678900"
}
```

### **2. Adicionar imóvel**

`POST /api/cadastro/{id}/imovel`

```json
{
  "area": 15,
  "endereco": "Fazenda Boa Vista"
}
```

### **3. Finalizar cadastro**

`POST /api/cadastro/{id}/finalizar`
**Resposta:**

```json
{
  "message": "Cadastro finalizado com sucesso!"
}
```

### **4. Consultar resumo do cadastro**

`GET /api/cadastro/{id}`

```json
{
  "nomeProprietario": "João da Silva",
  "documentoProprietario": "12345678900",
  "status": "Finalizado",
  "imoveis": [
    { "area": 15, "endereco": "Fazenda Boa Vista" }
  ],
  "valorTotal": 360000.00
}
```

### **5. Buscar proprietário por documento**

`GET /api/proprietario/{documento}`

```json
{
  "nome": "João da Silva",
  "documento": "12345678900",
  "imoveis": [
    { "area": 15, "endereco": "Fazenda Boa Vista" }
  ]
}
```

### **6. Calcular valor total dos imóveis**

`GET /api/proprietario/{documento}/valor-total`

```json
{
  "documento": "12345678900",
  "valorTotal": 360000.00
}
```

---

## **Tecnologias Usadas**

* **.NET 8** – API REST moderna com C#.
* **Entity Framework Core** – ORM para persistência.
* **SQLite** – Banco de dados leve para desenvolvimento.
* **Swagger** – Documentação e testes interativos da API.

---

## **Decisões de Arquitetura**

* **Arquitetura em camadas**: Controllers, Services, Repositories, Models e DTOs.
* **Por que não DDD/Clean Architecture?**

  * Como se trata de um **MVP simples e rápido**, **uma arquitetura em camadas bem definida é suficiente**.
  * Porém, a separação atual **permite evolução para Clean Architecture** futuramente sem grandes refatorações.
* **Tratamento global de exceções** via Middleware: retorna **códigos HTTP corretos** (400, 404, 500) com mensagens amigáveis.
* **Uso de DTOs**: evita ciclos de serialização e garante que apenas os dados necessários sejam expostos.

---

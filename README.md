# README

Este repositório contém um código desenvolvido em **C#** que automatiza a realização de um teste de digitação no site [10fastfingers](https://10fastfingers.com/typing-test/portuguese) usando **Selenium WebDriver** e realiza a inserção dos resultados desse teste em um banco de dados **PostgreSQL**. Além disso, é possível visualizar o último registro inserido no banco de dados através de uma **MessageBox**.

### Funcionalidades

- **Automatização do teste de digitação:** O código abre o navegador **Microsoft Edge**, acessa o site 10fastfingers, espera pelo carregamento da página e inicia o teste de digitação.

- **Inserção dos resultados no banco de dados:** Após a conclusão do teste, os resultados (palavras por minuto, quantidade de teclas pressionadas, precisão, palavras corretas e palavras erradas) são extraídos da página web e inseridos em uma tabela no banco de dados.

- **Visualização do último registro:** É possível visualizar o último registro inserido na tabela do banco de dados através de uma MessageBox, exibindo as métricas do último teste de digitação realizado.

### Requisitos

Para executar este código, é necessário ter instalado:

- **.NET Framework** (versão 5.0 ou superior)
- **Npgsql** (para conectar ao banco de dados PostgreSQL)
- **Selenium WebDriver** (para automação do navegador web)

### Execução

1. Certifique-se de ter instalado todos os requisitos mencionados acima.
2. Clone o repositório para o seu ambiente local.
3. Abra o projeto em um editor de código ou IDE compatível com .NET Framework.
4. Compile e execute o projeto.

### Configuração do Banco de Dados

Para configurar o banco de dados PostgreSQL, siga estes passos:

1. Instale o **PostgreSQL** em seu ambiente local, se ainda não estiver instalado.
2. Execute o script SQL fornecido (`Execution.sql`) para criar a tabela "Execution" no banco de dados.
3. Certifique-se de que as credenciais de conexão ao banco de dados estão corretas no código-fonte (`Process.cs`).

### Nota

Certifique-se de que o **Microsoft Edge WebDriver** esteja instalado em seu ambiente e configurado corretamente para o Selenium WebDriver.

Este código foi desenvolvido com fins educacionais e pode ser adaptado conforme necessário para atender aos requisitos específicos do projeto.

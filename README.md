# Camaleão
> É um Mock API que te permite trabalhar com responses dinâmicos com base em regras que você pré define.

[![CircleCI](https://circleci.com/gh/mundipagg/camaleao.svg?style=svg&circle-token=5a6f757966c5d66f3e845148331dc0da10620672)](https://circleci.com/gh/mundipagg/camaleao)

O camaleão trabalha com template, que é onde você define toda a estrutura do seu Mock. Nele, irá conter a rota que usará para efetuar a chamada, o padrão de requisição que é esperado, os possíveis responses que terá, as regras para definir qual será o response, o contexto para criar variáveis e poder trabalhar em cima de futuras requisições e também em outros templates e, por fim, as actions para executar ações globais antes de processar o response.

![](/img/camaleao.jpg) 

## Começando

Essas instruções farão com que você tenha uma cópia do projeto em execução na sua máquina local para fins de desenvolvimento e teste.

### Pré-requisitos

- Docker

### Instalando

1. Faça o clone deste projeto com `git clone https://github.com/mundipagg/camaleao.git`
2. Entre na pasta do projeto e navegue até ./devops
3. Build a imagem do docker `docker image build`

Levantar o container

```
PATH: camelao/devops
docker-compose up
```

Derrubar o container

```
docker-compose down
```

## Utilizando

Consulte a [Wiki](https://github.com/mundipagg/camaleao/wiki).

## Contribuindo

Por favor, leia [CONTRIBUTING](https://github.com/mundipagg/camaleao/blob/feature/documentacao/CONTRIBUTING.md) para detalhes sobre o nosso código de conduta, e o processo para enviar pull request para nós.

## Versionamento

Por favor, leia o [CHANGELOG](https://github.com/mundipagg/camaleao/blob/feature/documentacao/CHANGELOG.md) para mais detalhes sobre mudanças e versões.

## Autores

Veja a lista de [contribuidores](https://github.com/mundipagg/camaleao/contributors) que participaram deste projeto.

## Licença

[![license](https://img.shields.io/github/license/mashape/apistatus.svg)](LICENSE.md)

#!/bin/bash

echo "Executando todos os testes..."

# Criar diretório para resultados
mkdir -p TestResults

# Executar testes com cobertura
dotnet test --collect:"XPlat Code Coverage" --results-directory TestResults

# Gerar relatório
echo "Relatório de testes gerado em TestResults/"
echo "Verifique o arquivo coverage.cobertura.xml para detalhes da cobertura"

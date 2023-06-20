using System;
using System.Collections.Generic;

interface IStack<Dado>
{
  void Empilhar(Dado elemento);
  Dado Desempilhar();
  Dado OTopo();
  int Tamanho { get; }
  bool EstaVazia { get; }
  List<Dado> Listar();
}


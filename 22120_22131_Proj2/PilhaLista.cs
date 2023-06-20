using System;
using System.Collections.Generic;

public class PilhaLista<Dado> : IStack<Dado> where Dado : IComparable<Dado>
{
    int tamanho;
    NoLista<Dado> topo;
    public int Tamanho => tamanho;

    public bool EstaVazia => topo == null;

    public PilhaLista()
    {
        topo = null; // lista fica vazia
        tamanho = 0; // nenhum valor foi empilhado
    }
    public Dado Desempilhar()
    {
        if (EstaVazia)
            throw new Exception("Pilha vazia (underflow)!");

        Dado valorDoTopo = topo.Info;
        topo = topo.Prox; // descarta o primeiro nó da lista
        tamanho--;
        return valorDoTopo;
    }

    public void Empilhar(Dado elemento)
    {
        NoLista<Dado> novoNo = new NoLista<Dado>(elemento, topo);
        // var novoNo = new NoLista<Dado>(elemento, topo);
        topo = novoNo;
        tamanho++;
    }

    public List<Dado> Listar()
    {
        List<Dado> valoresEmpilhados = new List<Dado>();
        // var valoresEmpilhados = new List<Dado>();
        var atual = topo;
        while (atual != null)
        {
            valoresEmpilhados.Add(atual.Info);
            atual = atual.Prox;
        }
        return valoresEmpilhados;
    }

    public Dado OTopo()
    {
        if (EstaVazia)
            throw new Exception("Pilha vazia (underflow)!");
        return topo.Info;  // valor do topo
    }
}


﻿using System;

class NoDuplo<Dado>
    where Dado : IComparable<Dado>,
                    IRegistro<Dado>
{
    NoDuplo<Dado> ant;
    Dado info; // informação guardada no nó da lista
    NoDuplo<Dado> prox;

    public NoDuplo(Dado novoDado)
    {
        Info = novoDado;
    }

    public Dado Info { get => info; set => info = value; }
    internal NoDuplo<Dado> Ant { get => ant; set => ant = value; }
    internal NoDuplo<Dado> Prox { get => prox; set => prox = value; }
}
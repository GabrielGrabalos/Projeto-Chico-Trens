﻿using System;
using System.IO;


class Cidade : IComparable<Cidade>, IRegistro<Cidade>
{
    const int tamNome = 15,
              tamX = 6,
              tamY = 6;

    const int iniNome = 0,
              iniX = iniNome + tamNome,
              iniY = iniX + tamX;

    string nome;
    float x, y;

    public string Nome { get => nome; set => nome = value.PadRight(tamNome, ' ').Substring(0, tamNome); }
    public float X { get => x; set => x = value; }
    public float Y { get => y; set => y = value; }

    public Cidade(string nome, float x, float y)
    {
        Nome = nome;
        X = x;
        Y = y;
    }

    public Cidade()
    {

    }

    public int CompareTo(Cidade outro)
    {
        return nome.ToUpperInvariant().CompareTo(outro.nome.ToUpperInvariant());
    }

    public Cidade LerRegistro(StreamReader arquivo)
    {
        if (arquivo != null) // arquivo aberto?
        {
            string linha = arquivo.ReadLine();
            Nome = linha.Substring(iniNome, tamNome);
            X = float.Parse(linha.Substring(iniX, tamX));
            Y = float.Parse(linha.Substring(iniY));
            return this; // retorna o próprio objeto Contato, com os dados
        }

        return default(Cidade);
    }

    public void GravarRegistro(StreamWriter arq)
    {
        if (arq != null)  // arquivo de saída aberto?
        {
            arq.WriteLine(ParaArquivo());
        }
    }
    public string ParaArquivo()
    {
        return Nome + X.ToString().PadLeft(tamX, ' ') + Y.ToString().PadLeft(tamY, ' ');
    }

    public override string ToString()
    {
        return Nome + " " + X.ToString().PadLeft(tamX, ' ') + Y.ToString().PadLeft(tamY, ' ');
    }
}

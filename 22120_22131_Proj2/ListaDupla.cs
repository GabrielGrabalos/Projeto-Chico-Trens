using System;
using System.IO;
using System.Windows.Forms;

class ListaDupla<Dado> : IDados<Dado>
                where Dado : IComparable<Dado>, IRegistro<Dado>, new()
{
    NoDuplo<Dado> primeiro, ultimo, atual;
    int quantosNos;

    public ListaDupla()
    {
        primeiro = ultimo = atual = null;
        quantosNos = 0;
    }

    private Situacao situacaoAtual;

    public Situacao SituacaoAtual
    {
        get { return situacaoAtual; }
        set { situacaoAtual = value; }
    }

    public int PosicaoAtual
    {
        get
        {
            var noAtual = primeiro;
            int posicao = 0;

            while (noAtual != atual && noAtual != null)
            {
                noAtual = noAtual.Prox;
                posicao++;
            }

            return posicao;
        }
        set
        {
            PosicionarEm(value);
        }
    }

    public bool EstaNoInicio
    {
        get
        {
            return atual == primeiro;
        }
    }

    public bool EstaNoFim
    {
        get
        {
            return atual == ultimo;
        }
    }

    public bool EstaVazio => primeiro == null; // Verifica se o primeiro nó é nulo para determinar se a lista está vazia
    public int Tamanho => quantosNos; // Retorna a quantidade de nós na lista

    public void LerDados(string nomeArquivo)
    {
        SituacaoAtual = Situacao.navegando;

        StreamReader leitor = new StreamReader(nomeArquivo);

        while (!leitor.EndOfStream)
        {
            Dado dado = new Dado();

            this.IncluirAposFim(dado.LerRegistro(leitor));
        }

        leitor.Close();
    }

    public void GravarDados(string nomeArquivo)
    {
        StreamWriter escritor = new StreamWriter(nomeArquivo);

        var noAtual = primeiro;

        while (noAtual != null)
        {
            noAtual.Info.GravarRegistro(escritor);
            noAtual = noAtual.Prox;
        }

        escritor.Close();
    }

    public void PosicionarNoPrimeiro()
    {
        SituacaoAtual = Situacao.navegando;

        atual = primeiro;
    }

    public void RetrocederPosicao()
    {
        SituacaoAtual = Situacao.navegando;

        if (atual != null)
        {
            atual = atual.Ant;
        }
    }

    public void AvancarPosicao()
    {
        SituacaoAtual = Situacao.navegando;

        if (atual != null)
        {
            atual = atual.Prox;
        }
    }

    public void PosicionarNoUltimo()
    {
        SituacaoAtual = Situacao.navegando;

        atual = ultimo;
    }

    public void PosicionarEm(int posicaoDesejada)
    {
        SituacaoAtual = Situacao.navegando;

        if (posicaoDesejada >= 0 && posicaoDesejada < quantosNos)
        {
            atual = primeiro;

            for (int i = 0; i < posicaoDesejada; i++)
            {
                atual = atual.Prox;
            }
        }
    }

    public bool Existe(Dado procurado, out int ondeEsta)
    {
        SituacaoAtual = Situacao.pesquisando;

        ondeEsta = -1;
        int posicao = 0;
        var noAtual = primeiro;

        while (noAtual != null)
        {
            if (noAtual.Info.CompareTo(procurado) == 0)
            {
                ondeEsta = posicao;
                atual = noAtual;
                return true;
            }

            noAtual = noAtual.Prox;
            posicao++;
        }

        atual = null;
        return false;
    }

    public bool Excluir(Dado dadoAExcluir)
    {
        SituacaoAtual = Situacao.excluindo;

        var noAtual = primeiro;

        while (noAtual != null)
        {
            if (noAtual.Info.CompareTo(dadoAExcluir) == 0)
            {
                if (noAtual == primeiro)
                {
                    primeiro = noAtual.Prox;
                    if (primeiro != null)
                        primeiro.Ant = null;
                }
                else
                {
                    noAtual.Ant.Prox = noAtual.Prox;
                    if (noAtual.Prox != null)
                        noAtual.Prox.Ant = noAtual.Ant;
                    if (noAtual == ultimo)
                        ultimo = noAtual.Ant;
                }

                quantosNos--;
                return true;
            }

            noAtual = noAtual.Prox;
        }

        return false;
    }

    public bool IncluirNoInicio(Dado novoValor)
    {
        SituacaoAtual = Situacao.incluindo;

        var novoNo = new NoDuplo<Dado>(novoValor);

        if (EstaVazio)
        {
            primeiro = ultimo = novoNo;
        }
        else
        {
            novoNo.Prox = primeiro;
            primeiro.Ant = novoNo;
            primeiro = novoNo;
        }

        quantosNos++;
        return true;
    }

    public bool IncluirAposFim(Dado novoValor)
    {
        SituacaoAtual = Situacao.incluindo;

        var novoNo = new NoDuplo<Dado>(novoValor);

        if (EstaVazio)
        {
            primeiro = ultimo = novoNo;
        }
        else
        {
            ultimo.Prox = novoNo;
            novoNo.Ant = ultimo;
            ultimo = novoNo;
        }

        quantosNos++;
        return true;
    }

    public bool Incluir(Dado novoValor)
    {
        SituacaoAtual = Situacao.incluindo;

        var novoNo = new NoDuplo<Dado>(novoValor);

        if (EstaVazio)
        {
            primeiro = ultimo = novoNo;
        }
        else if (novoValor.CompareTo(primeiro.Info) < 0)
        {
            novoNo.Prox = primeiro;
            primeiro.Ant = novoNo;
            primeiro = novoNo;
        }
        else if (novoValor.CompareTo(ultimo.Info) > 0)
        {
            ultimo.Prox = novoNo;
            novoNo.Ant = ultimo;
            ultimo = novoNo;
        }
        else
        {
            var noAtual = primeiro;

            while (noAtual != null)
            {
                if (novoValor.CompareTo(noAtual.Info) < 0)
                {
                    novoNo.Prox = noAtual;
                    novoNo.Ant = noAtual.Ant;
                    noAtual.Ant.Prox = novoNo;
                    noAtual.Ant = novoNo;
                    break;
                }

                noAtual = noAtual.Prox;
            }
        }

        quantosNos++;
        return true;
    }

    public bool Incluir(Dado novoValor, int posicaoDeInclusao)
    {
        SituacaoAtual = Situacao.incluindo;

        if (posicaoDeInclusao < 0 || posicaoDeInclusao > quantosNos)
            return false;

        if (posicaoDeInclusao == 0)
            return IncluirNoInicio(novoValor);

        if (posicaoDeInclusao == quantosNos)
            return IncluirAposFim(novoValor);

        var novoNo = new NoDuplo<Dado>(novoValor);
        var noAtual = primeiro;
        int posicao = 0;

        while (posicao < posicaoDeInclusao - 1)
        {
            noAtual = noAtual.Prox;
            posicao++;
        }

        novoNo.Prox = noAtual.Prox;
        novoNo.Ant = noAtual;
        noAtual.Prox.Ant = novoNo;
        noAtual.Prox = novoNo;

        quantosNos++;
        return true;
    }

    public Dado this[int indice]
    {
        get
        {
            if (indice < 0 || indice >= quantosNos)
                throw new ArgumentOutOfRangeException("indice");

            var noAtual = primeiro;
            int posicao = 0;

            while (posicao < indice)
            {
                noAtual = noAtual.Prox;
                posicao++;
            }

            return noAtual.Info;
        }
        set
        {
            if (indice < 0 || indice >= quantosNos)
                throw new ArgumentOutOfRangeException("indice");

            var noAtual = primeiro;
            int posicao = 0;

            while (posicao < indice)
            {
                noAtual = noAtual.Prox;
                posicao++;
            }

            noAtual.Info = value;
        }
    }

    public Dado DadoAtual()
    {
        if (atual != null)
            return atual.Info;

        return default(Dado);
    }

    public void ExibirDados()
    {
        SituacaoAtual = Situacao.navegando;

        var noAtual = primeiro;

        while (noAtual != null)
        {
            Console.WriteLine(noAtual.Info.ToString());
            noAtual = noAtual.Prox;
        }
    }

    public void ExibirDados(ListBox lista)
    {
        SituacaoAtual = Situacao.navegando;

        var noAtual = primeiro;

        while (noAtual != null)
        {
            lista.Items.Add(noAtual.Info.ToString());
            noAtual = noAtual.Prox;
        }
    }

    public void ExibirDados(ComboBox lista)
    {
        SituacaoAtual = Situacao.navegando;

        lista.Items.Clear();
        var noAtual = primeiro;

        while (noAtual != null)
        {
            lista.Items.Add(noAtual.Info.ToString());
            noAtual = noAtual.Prox;
        }
    }

    public void ExibirDados(TextBox lista)
    {
        SituacaoAtual = Situacao.navegando;

        lista.Clear();
        var noAtual = primeiro;

        while (noAtual != null)
        {
            lista.AppendText(noAtual.Info.ToString());
            lista.AppendText(Environment.NewLine);
            noAtual = noAtual.Prox;
        }
    }

    public void Ordenar()
    {
        SituacaoAtual = Situacao.navegando;

        if (quantosNos <= 1)
            return;

        bool trocou;
        do
        {
            trocou = false;
            var noAtual = primeiro;

            while (noAtual != null && noAtual.Prox != null)
            {
                if (noAtual.Info.CompareTo(noAtual.Prox.Info) > 0)
                {
                    TrocarValores(noAtual, noAtual.Prox);
                    trocou = true;
                }

                noAtual = noAtual.Prox;
            }
        } while (trocou);
    }

    private void TrocarValores(NoDuplo<Dado> no1, NoDuplo<Dado> no2)
    {
        var temp = no1.Info;
        no1.Info = no2.Info;
        no2.Info = temp;
    }

}
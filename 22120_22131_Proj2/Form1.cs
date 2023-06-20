using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _22120_22131_Proj2
{
    public partial class Form1 : Form
    {
        ListaDupla<Cidade> cidades = new ListaDupla<Cidade>();
        Situacao situacao = Situacao.navegando;

        int[,] matrizCaminhos; // Matriz adjascente que será utilizada na busca dos caminhos.
        List<Ligacao> ligacoes; // Lista de ligações.
        List<List<int>> caminhosEncontrados; // Todos os caminhos encontrados do ponto A ao B.
        int melhorCaminhoIndex = -1; // Index do melhor caminho em caminhosEncontrados.

        public Form1()
        {
            InitializeComponent();

            // Fecha o formulário caso nenhum arquivo seja selecionado:
            if (dlgAbrir.ShowDialog() != DialogResult.OK)
            {
                Close();
                return;
            }

            LerArquivo(dlgAbrir.FileName); // Lê o arquivo com nomes das cidades.

            // Exibe a primeira cidade:
            cidades.PosicionarNoPrimeiro();
            ExibirCidadeAtual();

            LerArquivoCaminhos(); // Lê o arquivo de caminhos.

            AtualizarBotoes(); // Atualiza os botões baseado na situacao atual.
        }

        private void ExibirCidadeAtual()
        {
            Cidade cidade = cidades.DadoAtual();

            txtNome.Text = cidade.Nome.Trim();
            udX.Value = (decimal)cidade.X;
            udY.Value = (decimal)cidade.Y;

            lbRegistro.Text = $"Registro: {cidades.PosicaoAtual + 1}/{cidades.Tamanho}";
        }

        private void LerArquivo(string arquivo)
        {
            cidades.LerDados(arquivo);

            // Adiciona na lista:
            lbCidades.Items.Clear();
            lbCidades.Items.Add("Nome             X     Y");
            cidades.ExibirDados(lbCidades);
        }

        private void btnInicio_Click(object sender, EventArgs e)
        {
            situacao = Situacao.navegando;

            cidades.PosicionarNoPrimeiro();
            ExibirCidadeAtual();

            AtualizarBotoes();
        }

        private void btnAnterior_Click(object sender, EventArgs e)
        {
            situacao = Situacao.navegando;

            cidades.RetrocederPosicao();
            ExibirCidadeAtual();


            AtualizarBotoes();
        }

        private void btnProximo_Click(object sender, EventArgs e)
        {
            situacao = Situacao.navegando;

            cidades.AvancarPosicao();
            ExibirCidadeAtual();

            AtualizarBotoes();
        }

        private void btnUltimo_Click(object sender, EventArgs e)
        {
            situacao = Situacao.navegando;

            cidades.PosicionarNoUltimo();
            ExibirCidadeAtual();

            AtualizarBotoes();
        }

        private void btnProcurar_Click(object sender, EventArgs e)
        {
            // Coloca na situação de pesquisa:
            if (situacao != Situacao.pesquisando)
            {
                situacao = Situacao.pesquisando;
                txtNome.Focus();

                AtualizarBotoes();
                return;
            }

            // Faz a pesquisa:
            int posAnterior = cidades.PosicaoAtual;

            if (cidades.Existe(new Cidade(txtNome.Text, (float)udX.Value, (float)udY.Value), out int posicao))
            {
                cidades.PosicionarEm(posicao);

                ExibirCidadeAtual();

                lbCidades.Focus();

                situacao = Situacao.navegando;
            }
            else
            {
                MessageBox.Show("Dado não encontrado. Verifique se o nome fornecido foi devidamente digitado.");

                cidades.PosicionarEm(posAnterior);
            }

            AtualizarBotoes();
        }

        private void btnNovo_Click(object sender, EventArgs e)
        {
            situacao = Situacao.incluindo;

            txtNome.Text = "";
            udX.Value = 0;
            udY.Value = 0;

            txtNome.Focus();

            AtualizarBotoes();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            situacao = Situacao.navegando;

            lbCidades.Focus();

            ExibirCidadeAtual();

            AtualizarBotoes();
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            if (situacao == Situacao.editando)
            {
                Cidade cidade = cidades.DadoAtual();

                cidade.Nome = txtNome.Text.Trim();
                cidade.X = (float)udX.Value;
                cidade.Y = (float)udY.Value;
            }
            else
            {
                Cidade cidade = new Cidade(txtNome.Text, (float)udX.Value, (float)udY.Value);
                cidades.IncluirAposFim(cidade);
                cidades.Ordenar();

                cidades.Existe(cidade, out int pos);

                cidades.PosicionarEm(pos);

                ExibirCidadeAtual();
            }

            situacao = Situacao.navegando;

            // Atualiza a lista:
            lbCidades.Items.Clear();

            lbCidades.Items.Add("Nome             X     Y");
            cidades.ExibirDados(lbCidades);
            lbCidades.Focus();

            AtualizarBotoes();

            pbMapa.Invalidate();
        }

        private void btnExcluir_Click(object sender, EventArgs e)
        {
            situacao = Situacao.excluindo;

            if (MessageBox.Show(
                "Deseja realmente excluir este item?",
                "Exclusão",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                int posAnterior = cidades.PosicaoAtual;

                if (cidades.Excluir(cidades.DadoAtual()))
                {

                    lbCidades.Items.Clear();
                    lbCidades.Items.Add("Nome             X     Y");
                    cidades.ExibirDados(lbCidades);

                    try
                    {
                        cidades.PosicionarEm(posAnterior);
                    }
                    catch (Exception)
                    {
                        cidades.PosicionarNoPrimeiro();
                    }

                    ExibirCidadeAtual();
                }
                else
                {
                    MessageBox.Show("Falha ao tentar excluir cidade!", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            situacao = Situacao.navegando;

            AtualizarBotoes();
        }

        private void btnSair_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Deseja mesmo sair?", "Sair", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
            {
                Close();
            }
        }

        private void AtualizarBotoes()
        {
            if (situacao == Situacao.navegando || situacao == Situacao.excluindo) // Se o usuário estiver navegando
            {
                btnInicio.Enabled = true;
                btnProximo.Enabled = true;
                btnAnterior.Enabled = true;
                btnUltimo.Enabled = true;
                btnProcurar.Enabled = true;
                btnNovo.Enabled = true;
                btnCancelar.Enabled = false;
                btnSalvar.Enabled = false;
                btnExcluir.Enabled = true;
                btnSair.Enabled = true;
            }
            else if (situacao == Situacao.incluindo || situacao == Situacao.editando)
            {
                btnInicio.Enabled = false;
                btnProximo.Enabled = false;
                btnAnterior.Enabled = false;
                btnUltimo.Enabled = false;
                btnProcurar.Enabled = false;
                btnNovo.Enabled = true;
                btnCancelar.Enabled = true;
                btnSalvar.Enabled = true;
                btnExcluir.Enabled = false;
                btnSair.Enabled = true;
            }
            else if (situacao == Situacao.pesquisando)
            {
                btnInicio.Enabled = false;
                btnProximo.Enabled = false;
                btnAnterior.Enabled = false;
                btnUltimo.Enabled = false;
                btnProcurar.Enabled = true;
                btnNovo.Enabled = false;
                btnCancelar.Enabled = true;
                btnSalvar.Enabled = false;
                btnExcluir.Enabled = false;
                btnSair.Enabled = false;
            }

            if (cidades.EstaNoInicio)
            {
                btnInicio.Enabled = false;
                btnAnterior.Enabled = false;
            }
            if (cidades.EstaNoFim)
            {
                btnProximo.Enabled = false;
                btnUltimo.Enabled = false;
            }
        }

        // Transforma a situação em editando:
        private void pbMapa_MouseClick(object sender, MouseEventArgs e)
        {
            udX.Value = Math.Round((decimal)e.X / pbMapa.Width, 3);
            udY.Value = Math.Round((decimal)e.Y / pbMapa.Height, 3);

            if (situacao == Situacao.navegando)
            {
                situacao = Situacao.editando;

                AtualizarBotoes();
            }
        }

        private void txtNome_Enter(object sender, EventArgs e)
        {
            if (situacao == Situacao.navegando)
            {
                situacao = Situacao.editando;

                AtualizarBotoes();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            cidades.GravarDados(dlgAbrir.FileName);
        }

        private void pbMapa_Paint(object sender, PaintEventArgs e)
        {
            // Caso o usuário esteja na primeira guia:
            if (tabGuias.SelectedIndex == 0)
            {
                int posAtual = cidades.PosicaoAtual;
                cidades.PosicionarNoPrimeiro();

                Pen borda = new Pen(Color.Black);
                Brush preenchimento = new SolidBrush(Color.Yellow);

                do
                {
                    Cidade cidade = cidades.DadoAtual();

                    int pontoX = (int)(cidade.X * pbMapa.Bounds.Width) - 3;
                    int pontoY = (int)(cidade.Y * pbMapa.Bounds.Height) - 3;

                    e.Graphics.FillEllipse(preenchimento, pontoX, pontoY, 6, 6);
                    e.Graphics.DrawEllipse(borda, pontoX, pontoY, 6, 6);

                    cidades.AvancarPosicao();

                } while (!cidades.EstaNoFim);

                cidades.PosicionarEm(posAtual);

                borda.Dispose();
                preenchimento.Dispose();
            }

            // Caso esteja na aba de busca:
            else
            {
                Graphics graphics = e.Graphics;
                Pen pen = new Pen(Color.Black);

                foreach (Ligacao ligacao in ligacoes)
                {
                    Cidade cidade1 = pegarCidadePorNome(ligacao.IdCidadeOrigem);
                    int x1 = (int)(cidade1.X * pbMapa.Bounds.Width);
                    int y1 = (int)(cidade1.Y * pbMapa.Bounds.Height);

                    Cidade cidade2 = pegarCidadePorNome(ligacao.IdCidadeDestino);
                    int x2 = (int)(cidade2.X * pbMapa.Bounds.Width);
                    int y2 = (int)(cidade2.Y * pbMapa.Bounds.Height);

                    graphics.DrawLine(pen, x1, y1, x2, y2);
                }

                try
                {
                    if (dgvMelhorCaminho.SelectedCells.Count > 0 && dgvMelhorCaminho.Focused)
                    {
                        DesenharUmCaminho(caminhosEncontrados[melhorCaminhoIndex], graphics);
                    }
                    else if (dgvCaminhos.SelectedCells.Count > 0)
                    {
                        DesenharUmCaminho(caminhosEncontrados[dgvCaminhos.SelectedCells[0].RowIndex], graphics);
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Erro ao desenhar caminho. Verifique se os dados especificados estão crretos");
                }
            }
        }

        // Pega um objeto cidade pelo nome:
        private Cidade pegarCidadePorNome(string nome)
        {
            if(cidades.Existe(new Cidade(nome,0,0), out int pos))
            {
                cidades.PosicionarEm(pos);
                return cidades.DadoAtual();
            }

            return null;
        }


        // Busca:
        Dictionary<string, int> indicesCidades; // Associa cada cidade a um índice.

        private void LerArquivoCaminhos()
        {
            MessageBox.Show("Selecione o arquivo de caminhos");

            OpenFileDialog dlgAbrirCaminhos = new OpenFileDialog();
            dlgAbrirCaminhos.FilterIndex = 1;

            if (dlgAbrirCaminhos.ShowDialog() == DialogResult.OK)
            {
                StreamReader leitor = new StreamReader(dlgAbrirCaminhos.FileName);

                ligacoes = new List<Ligacao>();

                // Cria as ligações:
                for (int i = 0; !leitor.EndOfStream; i++)
                {
                    Ligacao ligacao = new Ligacao("", "", 0, 0, 0);
                    ligacoes.Add(ligacao.LerRegistro(leitor));
                }

                indicesCidades = new Dictionary<string, int>();

                // A partir
                int index = 0;
                foreach (Ligacao ligacao in ligacoes)
                {
                    if (!indicesCidades.ContainsKey(ligacao.IdCidadeOrigem))
                    {
                        indicesCidades.Add(ligacao.IdCidadeOrigem, index);
                        index++;
                    }
                    if (!indicesCidades.ContainsKey(ligacao.IdCidadeDestino))
                    {
                        indicesCidades.Add(ligacao.IdCidadeDestino, index);
                        index++;
                    }
                }

                int qtdCidades = indicesCidades.Count;
                int[,] caminhos = new int[qtdCidades, qtdCidades];

                foreach (Ligacao ligacao in ligacoes)
                {
                    int origem = indicesCidades[ligacao.IdCidadeOrigem];
                    int destino = indicesCidades[ligacao.IdCidadeDestino];

                    int distance = ligacao.Distancia;

                    caminhos[origem, destino] = distance;
                    caminhos[destino, origem] = distance;
                }

                foreach (string cidade in indicesCidades.Keys)
                {
                    cbOrigem.Items.Add(cidade);
                    cbDestino.Items.Add(cidade);
                }

                matrizCaminhos = caminhos;

                dlgAbrirCaminhos.Dispose();
            }
            else
            {
                Close();
            }
        }

        public List<List<int>> BuscarCaminhos(int origem, int destino, int[,] matrizAdjacencia)
        {
            int qtasCidades = matrizAdjacencia.GetLength(0);

            bool[] passou = new bool[qtasCidades];

            List<List<int>> caminhos = new List<List<int>>();

            Stack<int> pilha = new Stack<int>();

            void Backtracking(int cidadeAtual)
            {
                pilha.Push(cidadeAtual);
                passou[cidadeAtual] = true;

                if (cidadeAtual == destino)
                {
                    caminhos.Add(pilha.ToList());
                }
                else
                {
                    for (int proximaCidade = 0; proximaCidade < qtasCidades; proximaCidade++)
                    {
                        if (!passou[proximaCidade] && matrizAdjacencia[cidadeAtual, proximaCidade] != 0)
                        {
                            Backtracking(proximaCidade);
                            passou[proximaCidade] = false; // Resetting the visited flag for backtracking
                            pilha.Pop(); // Removing the current city from the stack before moving to the next city
                        }
                    }
                }
            }

            Backtracking(origem);
            return caminhos;
        }


        private void btnAcharCaminhos_Click(object sender, EventArgs e)
        {
            indicesCidades.TryGetValue(cbOrigem.Text, out int keyOrigem);
            indicesCidades.TryGetValue(cbDestino.Text, out int keyDestino);

            caminhosEncontrados = BuscarCaminhos(keyOrigem, keyDestino, matrizCaminhos);

            AdicionarNoDataGridView(caminhosEncontrados);
            AdicionarMelhorNoDataGridView(caminhosEncontrados);
        }

        private void AdicionarMelhorNoDataGridView(List<List<int>> caminhosEncontrados)
        {
            List<int> melhorCaminho = AcharMelhorCaminho(caminhosEncontrados);
            if (melhorCaminho != null)
            {
                // Add the best path to the DataGridView
                dgvMelhorCaminho.Rows.Clear();
                dgvMelhorCaminho.Columns.Clear();
                dgvMelhorCaminho.Columns.Add("Melhor Caminho", "Melhor Caminho");

                foreach (int cidade in melhorCaminho)
                {
                    DataGridViewRow row = new DataGridViewRow();
                    DataGridViewCell cell = new DataGridViewTextBoxCell();
                    cell.Value = cidade;
                    row.Cells.Add(cell);

                    dgvMelhorCaminho.Rows.Add(row);
                }
            }

            lbMelhorCaminho.Text = $"Melhor caminho: ({TamanhoCaminho(melhorCaminho)} km)";
        }

        private int TamanhoCaminho(List<int> caminho)
        {
            int distanciaTotal = 0;
            for (int i = 0; i < caminho.Count - 1; i++)
            {
                int cidadeOrigemIndex = caminho[i];
                int cidadeDestinoIndex = caminho[i + 1];

                Cidade cidadeOrigem = pegarCidadePorIndice(cidadeOrigemIndex);
                Cidade cidadeDestino = pegarCidadePorIndice(cidadeDestinoIndex);

                Ligacao ligacao = AcharLigacao(cidadeOrigem, cidadeDestino);

                distanciaTotal += ligacao.Distancia;
            }

            return distanciaTotal;
        }

        private List<int> AcharMelhorCaminho(List<List<int>> caminhosEncontrados)
        {
            List<int> melhorCaminho = null;
            int menorDistancia = int.MaxValue;
            int index = 0;
            int melhorIndex = 0;

            foreach (List<int> caminho in caminhosEncontrados)
            {
                int distanciaTotal = 0;
                for (int i = 0; i < caminho.Count - 1; i++)
                {
                    int cidadeOrigemIndex = caminho[i];
                    int cidadeDestinoIndex = caminho[i + 1];

                    Cidade cidadeOrigem = pegarCidadePorIndice(cidadeOrigemIndex);
                    Cidade cidadeDestino = pegarCidadePorIndice(cidadeDestinoIndex);

                    Ligacao ligacao = AcharLigacao(cidadeOrigem, cidadeDestino);

                    distanciaTotal += ligacao.Distancia;
                }

                if (distanciaTotal < menorDistancia)
                {
                    menorDistancia = distanciaTotal;
                    melhorCaminho = caminho;
                    melhorIndex = index;
                }

                index++;
            }

            melhorCaminhoIndex = melhorIndex;
            return melhorCaminho;
        }

        private Ligacao AcharLigacao(Cidade cidadeOrigem, Cidade cidadeDestino)
        {
            foreach (Ligacao ligacao in ligacoes)
            {
                if (ligacao.IdCidadeOrigem == cidadeOrigem.Nome && ligacao.IdCidadeDestino == cidadeDestino.Nome||
                    ligacao.IdCidadeOrigem == cidadeDestino.Nome && ligacao.IdCidadeDestino == cidadeOrigem.Nome)
                {
                    return ligacao;
                }
            }

            return null;
        }


        private void AdicionarNoDataGridView(List<List<int>> caminhos)
        {
            dgvCaminhos.Rows.Clear();
            dgvCaminhos.Columns.Clear();

            int maxCells = caminhos.Max(caminho => caminho.Count);

            for (int i = 0; i < maxCells; i++)
            {
                DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                dgvCaminhos.Columns.Add(column);
            }

            foreach (List<int> caminho in caminhos)
            {
                DataGridViewRow row = new DataGridViewRow();

                foreach (int cidade in caminho)
                {
                    DataGridViewCell cell = new DataGridViewTextBoxCell();
                    cell.Value = cidade;

                    row.Cells.Add(cell);
                }

                dgvCaminhos.Rows.Add(row);
            }
        }



        private void DesenharUmCaminho(List<int> caminho, Graphics g)
        {
            var pen = new Pen(Color.Red, 5);

            for (int i = 0; i < caminho.Count - 1; i++)
            {
                int cidadeOrigemIndex = caminho[i];
                int cidadeDestinoIndex = caminho[i + 1];

                Cidade cidadeOrigem = pegarCidadePorIndice(cidadeOrigemIndex);
                Cidade cidadeDestino = pegarCidadePorIndice(cidadeDestinoIndex);

                int x1 = (int)(cidadeOrigem.X * pbMapa.Width);
                int y1 = (int)(cidadeOrigem.Y * pbMapa.Height);
                int x2 = (int)(cidadeDestino.X * pbMapa.Width);
                int y2 = (int)(cidadeDestino.Y * pbMapa.Height);

                g.DrawLine(pen, x1, y1, x2, y2);
            }

            pen.Dispose();
        }

        private Cidade pegarCidadePorIndice(int cidadeOrigemIndex)
        {
            foreach (var cidade in indicesCidades)
            {
                if (cidade.Value == cidadeOrigemIndex)
                {
                    return pegarCidadePorNome(cidade.Key);
                }
            }

            return null;
        }

        private void tabGuias_SelectedIndexChanged(object sender, EventArgs e)
        {
            pbMapa.Invalidate();
        }

        private void dgvCaminhos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            pbMapa.Invalidate();
            lbCaminhoSelecionado.Text = $"Km do caminho selecionado: ({TamanhoCaminho(caminhosEncontrados[dgvCaminhos.SelectedCells[0].RowIndex])} km)";
        }

        private void dgvMelhorCaminho_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            pbMapa.Invalidate();
        }
    }
}

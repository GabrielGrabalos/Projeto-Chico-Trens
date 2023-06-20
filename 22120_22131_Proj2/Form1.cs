using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _22120_22131_Proj2
{
    public partial class Form1 : Form
    {
        ListaDupla<Cidade> cidades = new ListaDupla<Cidade>();
        Situacao situacao = Situacao.navegando;

        int[,] caminhos;

        public Form1()
        {
            InitializeComponent();

            if (dlgAbrir.ShowDialog() != DialogResult.OK)
            {
                Close();
                return;
            }

            LerArquivo(dlgAbrir.FileName);

            cidades.PosicionarNoPrimeiro();
            ExibirCidadeAtual();

            LerArquivoCaminhos();

            AtualizarBotoes();
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
            if (situacao != Situacao.pesquisando)
            {
                situacao = Situacao.pesquisando;
                txtNome.Focus();

                AtualizarBotoes();
                return;
            }

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

            borda.Dispose();
            preenchimento.Dispose();
        }

        // Busca:
        private void LerArquivoCaminhos()
        {
            MessageBox.Show("Selecione o arquivo de caminhos");

            OpenFileDialog dlgAbrirCaminhos = new OpenFileDialog();
            dlgAbrirCaminhos.FilterIndex = 1;

            if (dlgAbrirCaminhos.ShowDialog() == DialogResult.OK)
            {
                StreamReader leitor = new StreamReader(dlgAbrirCaminhos.FileName);

                List<Ligacao> ligacoes = new List<Ligacao>();

                for (int i = 0; !leitor.EndOfStream; i++)
                {
                    Ligacao ligacao = new Ligacao("", "", 0, 0, 0);
                    ligacoes.Add(ligacao.LerRegistro(leitor));
                }

                Dictionary<string, int> indicesCidades = new Dictionary<string, int>();

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
                    int origem  = indicesCidades[ligacao.IdCidadeOrigem];
                    int destino = indicesCidades[ligacao.IdCidadeDestino];

                    int distance = ligacao.Distancia;

                    caminhos[origem, destino] = distance;
                    caminhos[destino, origem] = distance;
                }


                dlgAbrirCaminhos.Dispose();
            }
            else
            {
                Close();
            }
        }
    }
}
